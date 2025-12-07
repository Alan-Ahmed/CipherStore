using Application.Mappings;
using Application.Services;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tests;

public class IntegrationTests
{
    // Helper för att skapa en unik "låtsas-databas" i minnet för varje test
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private IMapper GetMapper()
    {
        // Här laddar vi din riktiga Mapping-profil så vi vet att den funkar
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }

    // TEST 1: Skapa och Hämta (Integration)
    [Fact]
    public async Task CreateAndRetrieveProduct_IntegrationTest()
    {
        // Arrange - Sätt upp databas och service
        using var context = GetInMemoryDbContext();
        var repo = new ProductRepository(context);
        var service = new ProductService(repo, GetMapper());

        // Act - Skapa en produkt
        var created = await service.CreateProductAsync(new ProductDto
        {
            Name = "Integration YubiKey",
            Price = 500,
            Category = "Hardware",
            StockQuantity = 10
        });

        // Act - Hämta den igen
        var retrieved = await service.GetProductByIdAsync(created.Id);

        // Assert - Kolla att det är samma
        Assert.NotNull(retrieved);
        Assert.Equal("Integration YubiKey", retrieved.Name);
    }

    // TEST 2: Filtrering (VG-funktion)
    [Fact]
    public async Task FilterProducts_IntegrationTest()
    {
        // Arrange - Lägg in två produkter med olika kategori
        using var context = GetInMemoryDbContext();
        context.Products.AddRange(
            new Product { Name = "A", Category = "Hardware", IsActive = true },
            new Product { Name = "B", Category = "Software", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repo = new ProductRepository(context);
        var service = new ProductService(repo, GetMapper());

        // Act - Sök bara efter Hardware
        var result = await service.GetAllProductsAsync("Hardware");

        // Assert - Ska bara hitta 1
        Assert.Single(result);
        Assert.Equal("A", result[0].Name);
    }

    // TEST 3: Soft Delete (Kolla att IsActive blir false i DB)
    [Fact]
    public async Task DeleteProduct_SoftDelete_IntegrationTest()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var p = new Product { Name = "To Delete", IsActive = true };
        context.Products.Add(p);
        await context.SaveChangesAsync();

        var repo = new ProductRepository(context);
        var service = new ProductService(repo, GetMapper());

        // Act - Ta bort
        await service.DeleteProductAsync(p.Id);

        // Assert - Hämta direkt från DB och kolla "IsActive"
        var dbProduct = await context.Products.FindAsync(p.Id);
        Assert.NotNull(dbProduct);
        Assert.False(dbProduct.IsActive); // Ska vara false!
    }

    // TEST 4: GetAll ska inte visa borttagna
    [Fact]
    public async Task GetAll_IgnoresDeleted_IntegrationTest()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        context.Products.Add(new Product { Name = "Active", IsActive = true });
        context.Products.Add(new Product { Name = "Deleted", IsActive = false });
        await context.SaveChangesAsync();

        var repo = new ProductRepository(context);
        var service = new ProductService(repo, GetMapper());

        // Act
        var result = await service.GetAllProductsAsync();

        // Assert - Ska bara hitta den aktiva
        Assert.Single(result);
        Assert.Equal("Active", result[0].Name);
    }
}