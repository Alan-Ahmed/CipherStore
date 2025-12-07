using Application.DTOs;
using Application.Interfaces;
using Application.Validators;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IAppDbContext _context;
    private readonly IEmailService _emailService;

    public OrderService(IAppDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
    {
        // 1. Validera input
        var validationError = OrderValidator.Validate(dto);
        if (validationError != null) throw new Exception(validationError);

        // 2. Skapa Ordern
        var order = new Order
        {
            CustomerName = dto.CustomerName,
            CustomerEmail = dto.CustomerEmail,
            ShippingAddress = dto.ShippingAddress,
            Status = OrderStatus.New,
            OrderDate = DateTime.UtcNow
        };

        decimal totalAmount = 0;

        // 3. Loopa igenom varukorgen
        foreach (var item in dto.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null) throw new Exception($"Produkt med ID {item.ProductId} finns inte.");

            if (product.StockQuantity < item.Quantity)
                throw new Exception($"Inte tillräckligt lager för {product.Name}.");

            product.StockQuantity -= item.Quantity;

            var orderItem = new OrderItem
            {
                Product = product,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };

            order.OrderItems.Add(orderItem);
            totalAmount += (product.Price * item.Quantity);
        }

        order.TotalAmount = totalAmount;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.Id.ToString(),
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            CustomerEmail = order.CustomerEmail,
            CustomerName = order.CustomerName,
            ShippingAddress = order.ShippingAddress,
            // Mappa items även här för konsistens
            Items = order.OrderItems.Select(oi => new OrderItemDto
            {
                ProductName = oi.Product.Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };
    }

    public async Task<List<OrderDto>> GetAllOrdersAsync()
    {
        return await _context.Orders
            // VIKTIGT: Visa bara ordrar som INTE är "New" (dvs Betalda eller senare)
            .Where(o => o.Status != OrderStatus.New)
            .OrderByDescending(o => o.OrderDate)
            .Include(o => o.OrderItems)      // Hämta rader
            .ThenInclude(oi => oi.Product)   // Hämta produktinfo
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.Id.ToString(),
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                CustomerEmail = o.CustomerEmail,
                CustomerName = o.CustomerName,
                ShippingAddress = o.ShippingAddress,
                // Mappa produkterna till DTO
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            }).ToListAsync();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return null;

        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.Id.ToString(),
            OrderDate = order.OrderDate,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            CustomerEmail = order.CustomerEmail,
            CustomerName = order.CustomerName,
            ShippingAddress = order.ShippingAddress,
            // Mappa produkterna så de syns i Popupen
            Items = order.OrderItems.Select(oi => new OrderItemDto
            {
                ProductName = oi.Product.Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };
    }

    public async Task UpdateOrderStatusAsync(int orderId, string newStatusStr)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null) throw new Exception("Order hittades inte.");

        if (Enum.TryParse<OrderStatus>(newStatusStr, true, out var newStatus))
        {
            order.Status = newStatus;

            if (newStatus == OrderStatus.Shipped)
            {
                await _emailService.SendEmailAsync(
                    order.CustomerEmail,
                    $"Din order #{order.Id} har skickats!",
                    $"Hej {order.CustomerName}!\n\nDin order är nu packad och på väg.");
            }

            await _context.SaveChangesAsync();
        }
        else
        {
            throw new Exception($"Ogiltig status: {newStatusStr}");
        }
    }
}