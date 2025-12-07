using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // Alla får lägga en order
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(dto);
            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    // Bara admin får se alla ordrar
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    // --- HÄR ÄR DEN NYA METODEN (Hämta en specifik order) ---
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        try
        {
            // Vi ropar på servicen (som kan svara null om id är fel)
            var order = await _orderService.GetOrderByIdAsync(id);

            // Här använder vi null-kollen vi pratade om!
            if (order == null)
            {
                return NotFound(new { Message = $"Order med ID {id} hittades inte." });
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    // Bara admin får ändra status (Betald/Skickad)
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
    {
        try
        {
            await _orderService.UpdateOrderStatusAsync(id, status);
            return Ok(new { Message = $"Order status ändrad till {status}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}