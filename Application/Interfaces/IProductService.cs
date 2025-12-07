using Application.DTOs;

namespace Application.Interfaces;

public interface IProductService
{
    // VG-KRAV: Filtrering här också
    Task<List<ProductDto>> GetAllProductsAsync(string? category = null);
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(ProductDto productDto);
    Task DeleteProductAsync(int id);
    Task UpdateProductStockAsync(int id, int newQuantity);
}