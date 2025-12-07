using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CartItemDto
{
    // --- Dessa fält fanns troligen redan ---
    [Required]
    public int ProductId { get; set; }

    [Required]
    [Range(1, 100)] // Exempel på validering
    public int Quantity { get; set; }

    // --- DESSA FÄLT SAKNADES OCH MÅSTE LÄGGAS TILL ---
    // De behövs för att skapa Stripe Session och för att beräkna priset i OrderService.

    [Required]
    public string ProductName { get; set; } = string.Empty; // Fixar felet om ProductName

    [Required]
    public decimal UnitPrice { get; set; } // Fixar felet om UnitPrice
}