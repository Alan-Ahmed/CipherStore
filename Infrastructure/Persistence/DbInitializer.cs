using Domain.Entities;

namespace Infrastructure.Persistence;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        // Se till att databasen är skapad
        context.Database.EnsureCreated();

        // Kolla om det redan finns en Admin
        if (context.AdminUsers.Any())
        {
            return; // Databasen är redan seedad
        }

        // Skapa din första Admin-användare
        var admin = new AdminUser
        {
            Username = "admin",
            PasswordHash = "password", // I ett riktigt projekt: Hasha detta!
            TwoFactorSecretKey = "1234567890" // Enkel test-nyckel för 2FA
        };

        context.AdminUsers.Add(admin);

        // Lägg till några demo-produkter (Säkerhetstema)
        var products = new Product[]
        {
            new Product { Name = "YubiKey 5 NFC", Description = "Hårdvarunyckel för 2FA", Price = 550, StockQuantity = 50, ImageUrl = "yubikey.jpg", Category = "Hardware" },
            new Product { Name = "Faraday Bag", Description = "Blockerar all signalspårning", Price = 299, StockQuantity = 20, ImageUrl = "faraday.jpg", Category = "Privacy" },
            new Product { Name = "Webcam Cover", Description = "Skydd för kameran", Price = 49, StockQuantity = 100, ImageUrl = "cover.jpg", Category = "Privacy" }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}