using Application.DTOs;

namespace Application.Validators;

public static class OrderValidator
{
    public static string? Validate(CreateOrderDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CustomerName)) return "Namn saknas.";
        if (string.IsNullOrWhiteSpace(dto.CustomerEmail)) return "E-post saknas.";
        if (!dto.CustomerEmail.Contains("@")) return "Ogiltig e-post.";
        if (string.IsNullOrWhiteSpace(dto.ShippingAddress)) return "Adress saknas.";

        if (dto.Items == null || !dto.Items.Any()) return "Varukorgen är tom.";

        foreach (var item in dto.Items)
        {
            if (item.Quantity <= 0) return "Antal måste vara större än 0.";
        }

        return null; // Allt OK
    }
}