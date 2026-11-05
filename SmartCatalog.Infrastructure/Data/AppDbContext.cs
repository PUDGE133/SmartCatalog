using Microsoft.EntityFrameworkCore;
using SmartCatalog.Domain.Entities;
using SmartCatalog.Infrastructure.Configurations;

namespace SmartCatalog.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}