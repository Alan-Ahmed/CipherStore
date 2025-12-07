using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class AdminUser
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    // Här sparas hemligheten för Google Authenticator (VG-krav)
    public string? TwoFactorSecretKey { get; set; }
}