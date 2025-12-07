using Application.DTOs;

namespace Application.Interfaces;

public interface IOrderService
{
    // Skapa order (Publik)
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);

    // Hämta alla ordrar (Admin)
    Task<List<OrderDto>> GetAllOrdersAsync();

    // Hämta specifik order (Admin - För popup-rutan)
    // OBS: "?" betyder att den får returnera null om ordern inte finns
    Task<OrderDto?> GetOrderByIdAsync(int id);

    // Uppdatera status (Admin: Betald/Skickad)
    Task UpdateOrderStatusAsync(int orderId, string status);
}