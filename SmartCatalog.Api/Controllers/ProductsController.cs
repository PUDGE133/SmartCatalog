using Microsoft.AspNetCore.Mvc;
using SmartCatalog.Api.Contracts;
using SmartCatalog.Infrastructure.Entities;
using SmartCatalog.Infrastructure.Repositories;

namespace SmartCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetAll(CancellationToken ct)
        => Ok(await repo.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Product>> GetById(Guid id, CancellationToken ct)
    {
        var product = await repo.GetByIdAsync(id, ct);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(CreateProductRequest request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description ?? string.Empty,
            Price = request.Price,
            Stock = request.Stock,
            CreatedAt = now,
            UpdatedAt = now
        };

        await repo.AddAsync(product, ct);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductRequest request, CancellationToken ct)
    {
        var updated = await repo.UpdateAsync(id, p =>
        {
            p.Name = request.Name;
            p.Description = request.Description ?? string.Empty;
            p.Price = request.Price;
            p.Stock = request.Stock;
        }, ct);

        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await repo.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}