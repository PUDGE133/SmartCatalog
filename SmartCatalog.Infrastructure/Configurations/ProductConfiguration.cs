using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCatalog.Infrastructure.Entities;

namespace SmartCatalog.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(nameof(AppDbContext.Products));

        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Id);

        builder.Property(p => p.Id).HasColumnOrder(1);
        builder.Property(p => p.Name).HasColumnOrder(2);
        builder.Property(p => p.Description).HasColumnOrder(3);
        builder.Property(p => p.Price).HasColumnOrder(4);
        builder.Property(p => p.Stock).HasColumnOrder(5);
        builder.Property(p => p.CreatedAt).HasColumnOrder(6);
        builder.Property(p => p.UpdatedAt).HasColumnOrder(7);
        builder.Property(p => p.Version).IsRowVersion().HasColumnOrder(8);
    }
}

