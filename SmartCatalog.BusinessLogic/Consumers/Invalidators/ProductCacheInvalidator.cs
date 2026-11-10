using SmartCatalog.Infrastructure.Cache;

namespace SmartCatalog.BusinessLogic.Consumers.Invalidators;

public class ProductCacheInvalidator(ICacheService cache)
{
    public Task InvalidateProductAsync(Guid productId, CancellationToken ct = default)
    {
        return cache.RemoveAsync(CacheKeys.GetProductKey(productId), ct);
    }

    public Task InvalidateProductListsAsync(CancellationToken ct = default)
    {
        return cache.IncrementVersionListsAsync(CacheKeys.ProductListVersionKey, ct);
    }

    public Task InvalidateAllAsync(Guid productId, CancellationToken ct = default)
    {
        return Task.WhenAll(InvalidateProductAsync(productId, ct), InvalidateProductListsAsync(ct));
    }
}
