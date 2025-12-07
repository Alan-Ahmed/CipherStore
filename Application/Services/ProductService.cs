using AutoMapper;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    // HÄR VAR FELET: Vi saknade "string? category = null"
    public async Task<List<ProductDto>> GetAllProductsAsync(string? category = null)
    {
        // 1. Hämta filtrerat från repository
        var products = await _productRepository.GetAllAsync(category);

        // 2. Använd AutoMapper för att göra om till DTOs
        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return null;

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateProductAsync(ProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        product.IsActive = true;

        await _productRepository.AddAsync(product);

        return _mapper.Map<ProductDto>(product);
    }
    public async Task DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product != null)
        {
            await _productRepository.DeleteAsync(product);
        }
    }
    // Lägg till denna metod för att uppdatera lagersaldot
    public async Task UpdateProductStockAsync(int id, int newQuantity)
    {
        // 1. Hämta produkten via repository
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            throw new Exception($"Produkt med ID {id} hittades inte.");

        // 2. Uppdatera antalet i minnet
        product.StockQuantity = newQuantity;

        // 3. Spara ändringen via repository
        // (Jag utgår från att ditt repository har en UpdateAsync, se punkt 3 nedan om det kraschar)
        await _productRepository.UpdateAsync(product);
    }
}