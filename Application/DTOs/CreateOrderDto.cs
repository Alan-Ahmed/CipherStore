namespace Application.DTOs;

public class CreateOrderDto
{
    // Kundinfo
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;

    // Listan med produkter och antal
    // OBS! CartItemDto måste vara definierad i en egen, separat fil!
    public List<CartItemDto> Items { get; set; } = new();
}