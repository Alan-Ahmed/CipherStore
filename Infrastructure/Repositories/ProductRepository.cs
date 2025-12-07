using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    // Uppdaterad för VG-krav: Server-side filtrering
    public async Task<List<Product>> GetAllAsync(string? category = null)
    {
        // 1. Börja med att hämta alla aktiva produkter
        var query = _context.Products.Where(p => p.IsActive);

        // 2. Om en kategori skickades med, lägg till ett filter (WHERE)
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category == category);
        }

        // 3. Kör frågan mot databasen
        return await query.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        // Soft delete - vi döljer den istället för att radera helt
        product.IsActive = false;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}