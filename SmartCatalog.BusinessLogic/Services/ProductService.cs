using FluentResults;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SmartCatalog.BusinessLogic.Extensions;
using SmartCatalog.BusinessLogic.FluentResults.Errors;
using SmartCatalog.BusinessLogic.FluentResults.Successes;
using SmartCatalog.BusinessLogic.Validators;
using SmartCatalog.Contracts.Events;
using SmartCatalog.Contracts.Requests;
using SmartCatalog.Infrastructure.Cache;
using SmartCatalog.Infrastructure.Entities;
using SmartCatalog.Infrastructure.Enums;
using SmartCatalog.Infrastructure.Repositories;

namespace SmartCatalog.BusinessLogic.Services;

public interface IProductService
{
    Task<Result<Product>> PostProductAsync(ProductRequest request, CancellationToken ct = default);
    Task<Result<Product>> GetProductAsync(Guid id, CancellationToken ct = default);
    Task<Result<IReadOnlyList<Product>>> GetListProductsAsync(int skip = 0, int take = int.MaxValue, CancellationToken ct = default);
    Task<Result<Product>> PutProductAsync(Guid id, ProductRequest request, CancellationToken ct = default);
    Task<Result> DeleteProductAsync(Guid id, CancellationToken ct = default);
}

public class ProductService(ICacheService cache, IProductRepository productRepository, IPublishEndpoint publishEndpoint) : IProductService
{
    private static readonly ProductRequestValidator _validator = new();

    public async Task<Result<Product>> PostProductAsync(ProductRequest request, CancellationToken ct = default)
    {
        var validateResult = await _validator.ValidateAsync(request, ct);
        if (!validateResult.IsValid)
            return Result.Fail(new BadRequestError(validateResult.GetFailReport()));

        var product = new Product(request, DateTime.UtcNow);
        await productRepository.PostProductAsync(product, ct);

        // Уведомляем RabbitMQ, что нужно сбросить кэш списков
        await publishEndpoint.Publish(new ProductCreatedEvent(product.Id), ct);

        return Result.Ok(product);
    }

    public async Task<Result<Product>> GetProductAsync(Guid id, CancellationToken ct = default)
    {
        var cacheKey = CacheKeys.GetProductKey(id);

        var cached = await cache.GetAsync<Product>(cacheKey, ct);
        if (cached is not null)
            return Result.Ok(cached);

        var product = await productRepository.GetProductAsync(id, Tracking.No, ct);
        if (product is null)
            return Result.Fail(new NotFoundError(Responses.NotFound));

        await cache.SetAsync(cacheKey, product, CacheKeys.ProductTtl, ct);
        return Result.Ok(product);
    }

    public async Task<Result<IReadOnlyList<Product>>> GetListProductsAsync(int skip = 0, int take = int.MaxValue, CancellationToken ct = default)
    {
        if (int.IsNegative(skip) || int.IsNegative(take))
            return Result.Fail(new BadRequestError(Responses.NegativeArgs));

        var version = await GetListVersionAsync(ct);
        var cacheKey = CacheKeys.GetProductListKey(version, skip, take);

        var cached = await cache.GetAsync<List<Product>>(cacheKey, ct);
        if (cached is not null)
            return Result.Ok<IReadOnlyList<Product>>(cached);

        var listProducts = await productRepository.GetListProductsAsync(Tracking.No, skip, take, ct);
        await cache.SetAsync(cacheKey, listProducts, CacheKeys.ListTtl, ct);

        return Result.Ok(listProducts);
    }

    public async Task<Result<Product>> PutProductAsync(Guid id, ProductRequest request, CancellationToken ct = default)
    {
        var product = await productRepository.GetProductAsync(id, Tracking.Yes, ct);
        if (product is null)
            return Result.Fail(new NotFoundError(Responses.NotFound));

        product.Update(request);
        try
        {
            await productRepository.PutProductAsync(product, request.Version, ct);

            // Уведомляем RabbitMQ
            await publishEndpoint.Publish(new ProductUpdatedEvent(product.Id), ct);

            return Result.Ok(product);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail(new ConflictError(Responses.ConflictVersions).WithMetadata(nameof(Product), product));
        }
    }

    public async Task<Result> DeleteProductAsync(Guid id, CancellationToken ct = default)
    {
        var product = await productRepository.GetProductAsync(id, Tracking.Yes, ct);
        if (product is null)
            return Result.Fail(new NotFoundError(Responses.NotFound));

        await productRepository.DeleteProductAsync(product, ct);

        // Уведомляем RabbitMQ
        await publishEndpoint.Publish(new ProductDeletedEvent(id), ct);

        return Result.Ok().WithSuccess(new OkValueSuccess(Responses.Delete));
    }

    private Task<long> GetListVersionAsync(CancellationToken ct)
    {
        // Это один вызов, который гарантирует, что версия всегда будет инициализирована (1) и никогда не прыгнет на 10 из-за гонки потоков.
        return cache.GetOrAddCounterAsync(CacheKeys.ProductListVersionKey, 1, ct);
    }
}