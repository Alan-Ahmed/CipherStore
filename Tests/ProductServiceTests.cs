using Application.DTOs;
using Application.Services;
using Application.Interfaces;
using Domain.Entities;
using Moq;
using Xunit;
using AutoMapper;

namespace Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new ProductService(_mockRepo.Object, _mockMapper.Object);
    }

    // --- TEST 1: Hämta alla (ska returnera lista) ---
    [Fact]
    public async Task GetAllProducts_ShouldReturnList_WhenProductsExist()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetAllAsync(null))
                 .ReturnsAsync(new List<Product> { new Product(), new Product() });
        _mockMapper.Setup(m => m.Map<List<ProductDto>>(It.IsAny<List<Product>>()))
                   .Returns(new List<ProductDto> { new ProductDto(), new ProductDto() });

        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    // --- TEST 2: Hämta en (ska hitta rätt ID) ---
    [Fact]
    public async Task GetProductById_ShouldReturnProduct_WhenExists()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetByIdAsync(1))
                 .ReturnsAsync(new Product { Id = 1, Name = "Test" });
        _mockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                   .Returns(new ProductDto { Id = 1, Name = "Test" });

        // Act
        var result = await _service.GetProductByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    // --- TEST 3: Hämta en som inte finns (ska bli null) ---
    [Fact]
    public async Task GetProductById_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetByIdAsync(99))
                 .ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetProductByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    // --- TEST 4: Skapa produkt (ska anropa AddAsync) ---
    [Fact]
    public async Task CreateProduct_ShouldCallAddAsync()
    {
        // Arrange
        var dto = new ProductDto { Name = "New Product" };
        var product = new Product { Name = "New Product" };

        _mockMapper.Setup(m => m.Map<Product>(dto)).Returns(product);
        _mockMapper.Setup(m => m.Map<ProductDto>(product)).Returns(dto);

        // Act
        await _service.CreateProductAsync(dto);

        // Assert
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    // --- TEST 5: Ta bort produkt (ska anropa DeleteAsync) ---
    [Fact]
    public async Task DeleteProduct_ShouldCallDeleteAsync_WhenProductExists()
    {
        // Arrange
        var product = new Product { Id = 1 };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        await _service.DeleteProductAsync(1);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(product), Times.Once);
    }

    // --- NYTT TEST 6: Ta bort produkt som inte finns (Ska INTE krascha/anropa delete) ---
    [Fact]
    public async Task DeleteProduct_ShouldDoNothing_WhenProductDoesNotExist()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        // Act
        await _service.DeleteProductAsync(99);

        // Assert - Verify att DeleteAsync ALDRIG kördes
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Product>()), Times.Never);
    }

    // --- NYTT TEST 7: Uppdatera lagersaldo (Ska spara nytt värde) ---
    [Fact]
    public async Task UpdateProductStock_ShouldUpdate_WhenProductExists()
    {
        // Arrange
        var product = new Product { Id = 1, StockQuantity = 10 };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        await _service.UpdateProductStockAsync(1, 50);

        // Assert
        Assert.Equal(50, product.StockQuantity); // Kolla att objektet ändrades
        _mockRepo.Verify(r => r.UpdateAsync(product), Times.Once); // Kolla att det sparades
    }

    // --- NYTT TEST 8: Uppdatera lagersaldo på icke-existerande (Ska kasta fel) ---
    [Fact]
    public async Task UpdateProductStock_ShouldThrowException_WhenProductNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.UpdateProductStockAsync(99, 10));
    }
}