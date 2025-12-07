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
        var validationError = OrderValidator.Validate(dto);
        if (validationError != null) throw new Exception(validationError);

        var order = new Order
        {
            CustomerName = dto.CustomerName,
            CustomerEmail = dto.CustomerEmail,
            ShippingAddress = dto.ShippingAddress,
            Status = OrderStatus.New,
            OrderDate = DateTime.UtcNow
        };

        decimal totalAmount = 0;

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
            ShippingAddress = order.ShippingAddress
        };
    }

    public async Task<List<OrderDto>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.Id.ToString(),
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                CustomerEmail = o.CustomerEmail,
                CustomerName = o.CustomerName,
                ShippingAddress = o.ShippingAddress
            }).ToListAsync();
    }

    // --- HÄR VAR FELET ---
    // Jag har lagt till "?" efter OrderDto för att matcha interfacet.
    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        // Om ordern inte finns returnerar vi null (det är nu tillåtet)
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
            ShippingAddress = order.ShippingAddress
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