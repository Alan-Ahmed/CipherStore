using Domain.Entities;

namespace Application.Interfaces;

public interface IProductRepository
{
    // Måste matcha din Repository-kod:
    Task<List<Product>> GetAllAsync(string? category = null);

    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}