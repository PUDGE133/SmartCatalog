using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using SmartCatalog.BusinessLogic.Services;
using SmartCatalog.Contracts.Requests;
using System.ComponentModel.DataAnnotations;

namespace SmartCatalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostProductAsync([FromBody, Required] ProductRequest request, CancellationToken ct = default)
    {
        return await productService.PostProductAsync(request, ct).ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetProductAsync([FromQuery, Required] Guid id, CancellationToken ct = default)
    {
        return await productService.GetProductAsync(id, ct).ToActionResult();
    }

    [HttpGet("List")]
    public async Task<IActionResult> GetListProductsAsync([FromQuery, Required] int skip = 0, [FromQuery, Required] int take = int.MaxValue, CancellationToken ct = default)
    {
        return await productService.GetListProductsAsync(skip, take, ct).ToActionResult();
    }

    [HttpPut]
    public async Task<IActionResult> PutProductAsync([FromQuery, Required] Guid id, [FromBody, Required] ProductRequest request, CancellationToken ct = default)
    {
        return await productService.PutProductAsync(id, request, ct).ToActionResult();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteProductAsync([FromQuery, Required] Guid id, CancellationToken ct = default)
    {
        return await productService.DeleteProductAsync(id, ct).ToActionResult();
    }
}