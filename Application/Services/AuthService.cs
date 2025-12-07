using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Authenticator;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(IAppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<string> LoginAsync(string username, string password, string twoFactorCode)
    {
        // 1. Hitta användaren (case-insensitive)
        var user = await _context.AdminUsers
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

        if (user == null)
            throw new Exception("Användaren hittades inte.");

        if (user.PasswordHash != password)
            throw new Exception("Fel lösenord.");

        // --- 🚨 NÖDLÖSNING / BAKDÖRR 🚨 ---
        // Om vi skriver "123456" så struntar vi i Google Auth-kollen.
        if (twoFactorCode == "123456")
        {
            return GenerateJwtToken(user);
        }
        // ------------------------------------

        // 2. Validera 2FA på riktigt
        if (!string.IsNullOrEmpty(user.TwoFactorSecretKey))
        {
            string cleanCode = twoFactorCode.Replace(" ", "").Trim();
            var tfa = new TwoFactorAuthenticator();
            bool isCorrectPIN = tfa.ValidateTwoFactorPIN(user.TwoFactorSecretKey, cleanCode);

            if (!isCorrectPIN) throw new Exception("Fel 2FA-kod.");
        }

        return GenerateJwtToken(user);
    }

    public async Task<string> RegisterAdminAsync(string username, string password)
    {
        if (await _context.AdminUsers.AnyAsync(u => u.Username == username))
            throw new Exception("Användarnamnet upptaget.");

        // Generera korrekt Base32-nyckel
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var random = new Random();
        var result = new char[16];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = chars[random.Next(chars.Length)];
        }
        string secretKey = new string(result);

        var user = new AdminUser
        {
            Username = username,
            PasswordHash = password,
            TwoFactorSecretKey = secretKey
        };

        _context.AdminUsers.Add(user);
        await _context.SaveChangesAsync();

        return secretKey;
    }

    private string GenerateJwtToken(AdminUser user)
    {
        // 👇 HÄR VAR FELET: Nu matchar nyckeln den i Program.cs!
        var jwtKey = _configuration["Jwt:Key"] ?? "super_secret_key_12345_must_be_long_enough_to_be_secure";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("Id", user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "CipherStore",
            audience: "CipherStoreUser",
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}