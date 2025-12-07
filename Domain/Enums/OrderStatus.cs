namespace Domain.Enums;

public enum OrderStatus
{
    New = 0,      // Ny inkommen
    Paid = 1,     // Markera Betald
    Packed = 2,   // Klar och Packad
    Shipped = 3   // Skickad (Här skickas mejlet)
}