using Microsoft.EntityFrameworkCore;
using SmartCatalog.Infrastructure.Data;
using SmartCatalog.Infrastructure.Entities;

namespace SmartCatalog.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Product> AddAsync(Product product, CancellationToken ct = default);
    Task<bool> UpdateAsync(Guid id, Action<Product> applyChanges, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default) =>
        await db.Products.AsNoTracking().ToListAsync(ct);

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Product> AddAsync(Product product, CancellationToken ct = default)
    {
        db.Products.Add(product);
        await db.SaveChangesAsync(ct);
        return product;
    }

    public async Task<bool> UpdateAsync(Guid id, Action<Product> applyChanges, CancellationToken ct = default)
    {
        // ВАЖНО: без AsNoTracking — нужен tracking, чтобы SaveChanges увидел изменения
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (product is null) return false;

        applyChanges(product);
        product.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (product is null) return false;

        db.Products.Remove(product);
        await db.SaveChangesAsync(ct);
        return true;
    }
}