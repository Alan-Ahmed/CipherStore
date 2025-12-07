namespace Application.Interfaces;

public interface IAuthService
{
    // Returnerar en JWT-token om inloggningen lyckas
    Task<string> LoginAsync(string username, string password, string twoFactorCode);

    // Hjälpmetod för att skapa en ny Admin och få QR-kod setup (körs en gång)
    Task<string> RegisterAdminAsync(string username, string password);
}