using SmartCatalog.Contracts.Requests;

namespace SmartCatalog.Infrastructure.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public uint Version { get; set; }

    public Product() { }

    public Product(ProductRequest request, DateTime createdDateTime)
    {
        Name = request.Name;
        Description = request.Description;
        Price = request.Price;
        Stock = request.Stock;
        CreatedAt = createdDateTime;
        UpdatedAt = createdDateTime;
    }

    public void Update(ProductRequest request)
    {
        Name = request.Name;
        Description = request.Description;
        Price = request.Price;
        Stock = request.Stock;
        UpdatedAt = DateTime.UtcNow;
    }
}