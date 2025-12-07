using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Order
{
    [Key]
    public int Id { get; set; } // <-- Denna kommer automatiskt bli 1, 2, 3...

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    // TA BORT raden "OrderNumber". Vi behöver den inte längre.
    // Vi använder "Id" som ordernummer istället.

    [Required]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required]
    public string ShippingAddress { get; set; } = string.Empty;

    // Se till att din Enum har rätt steg (se punkt 2 nedan)
    public OrderStatus Status { get; set; } = OrderStatus.New;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public string? StripeSessionId { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new();
}