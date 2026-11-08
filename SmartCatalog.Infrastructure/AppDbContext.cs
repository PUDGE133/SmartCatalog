using Microsoft.EntityFrameworkCore;
using SmartCatalog.Infrastructure.Configurations;
using SmartCatalog.Infrastructure.Entities;

namespace SmartCatalog.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}