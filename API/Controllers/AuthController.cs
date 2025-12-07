using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var token = await _authService.LoginAsync(dto.Username, dto.Password, dto.TwoFactorCode);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { Message = ex.Message });
        }
    }

    // Kör denna en gång via Swagger för att skapa din Admin
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin(string username, string password)
    {
        try
        {
            var secretKey = await _authService.RegisterAdminAsync(username, password);
            return Ok(new { Message = "Admin skapad!", TwoFactorSecret = secretKey });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}