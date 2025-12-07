using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")] // Viktigt för valuta i SQL
    public decimal Price { get; set; }

    public int StockQuantity { get; set; } // Används för logiken "Slut i lager"

    public string ImageUrl { get; set; } = string.Empty;

    public string Category { get; set; } = "Hardware"; // T.ex. "Privacy", "Hardware"

    public bool IsActive { get; set; } = true; // Så vi slipper ta bort produkter permanent
}