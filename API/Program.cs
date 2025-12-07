using Application.Interfaces;
using Application.Services;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authorization; // Ny using: Behövs för att kunna använda Authorize

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURATION & SERVICES ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration (Oförändrad - ser bra ut!)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CipherStore API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Skriv: 'Bearer {din_token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// Databas
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>()!);

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Services & Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// --- JWT AUTHENTICATION FIX ---
// Vi använder en fallback om nyckeln saknas
var jwtKey = builder.Configuration["Jwt:Key"] ?? "super_secret_key_12345_must_be_long_enough_to_be_secure";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});

// 👇 CORS: Tillåt allt för att undvika problem
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// --- Authorization Policy (Valfritt men bra för Admin-roller)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});
// --- SLUT PÅ SERVICES ---

var app = builder.Build();

// --- 2. PIPELINE ---

app.UseMiddleware<ExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // DbInitializer.Initialize(context); 
    }
    catch (Exception ex)
    {
        Console.WriteLine($"DB Error: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Avstängd

// 👇 1. VIKTIGT: Aktivera CORS (Måste ligga före routing/auth)
app.UseCors("AllowAll");

// 👇 2. VIKTIGT: Authentication MÅSTE ligga före Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }