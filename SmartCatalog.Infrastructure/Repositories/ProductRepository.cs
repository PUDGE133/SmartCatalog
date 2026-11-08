using Microsoft.EntityFrameworkCore;
using SmartCatalog.Infrastructure.Entities;
using SmartCatalog.Infrastructure.Enums;
using SmartCatalog.Infrastructure.Extensions;

namespace SmartCatalog.Infrastructure.Repositories;

public interface IProductRepository
{
    Task PostProductAsync(Product product, CancellationToken ct = default);
    Task<Product?> GetProductAsync(Guid id, Tracking tracking, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetListProductsAsync(Tracking tracking, int skip = 0, int take = int.MaxValue, CancellationToken ct = default);
    Task PutProductAsync(Product product, uint version, CancellationToken ct = default);
    Task DeleteProductAsync(Product product, CancellationToken ct = default);
}

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task PostProductAsync(Product product, CancellationToken ct = default)
    {
        await context.Products.AddAsync(product, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<Product?> GetProductAsync(Guid id, Tracking tracking, CancellationToken ct = default)
    {
        return await context.Products
            .BuildQueryBase(tracking)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Product>> GetListProductsAsync(Tracking tracking, int skip = 0, int take = int.MaxValue, CancellationToken ct = default)
    {
        return await context.Products
            .BuildQueryBase(tracking)
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task PutProductAsync(Product product, uint version, CancellationToken ct = default)
    {
        context.SetOriginalVersion(product, nameof(Product.Version), version);
        context.Products.Update(product);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteProductAsync(Product product, CancellationToken ct = default)
    {
        context.Products.Remove(product);
        await context.SaveChangesAsync(ct);
    }
}