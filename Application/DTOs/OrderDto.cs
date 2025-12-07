using System;
using System.Collections.Generic;

namespace Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;

    public List<OrderItemDto> Items { get; set; } = new();
}