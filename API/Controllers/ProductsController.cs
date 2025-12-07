using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // VG-KRAV: Server-side filtrering
    // Nu kan du anropa: GET /api/products?category=Hardware
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? category)
    {
        // Vi skickar kategorin vidare till servicen (som skickar till repot)
        var products = await _productService.GetAllProductsAsync(category);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    // Endast Admin får skapa produkter (Kräver Token)
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductDto dto)
    {
        var created = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // Vi kollar först om produkten finns (bra praxis)
        var existing = await _productService.GetProductByIdAsync(id);
        if (existing == null) return NotFound();

        await _productService.DeleteProductAsync(id);

        return NoContent(); // 204 No Content är standard vid lyckad delete
    }
    // PUT: api/products/{id}/stock
    [HttpPut("{id}/stock")]
    // [Authorize(Roles = "Admin")] // Avkommentera när du vill slå på säkerheten
    public async Task<IActionResult> UpdateStock(int id, [FromBody] int newQuantity)
    {
        try
        {
            await _productService.UpdateProductStockAsync(id, newQuantity);
            return Ok(new { message = "Lagersaldo uppdaterat till " + newQuantity });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
