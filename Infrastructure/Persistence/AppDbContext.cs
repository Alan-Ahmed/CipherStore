using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Infrastructure.Persistence;

// Denna klass ärver från DbContext (EF Core) OCH implementerar ditt Interface
public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<AdminUser> AdminUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Konfigurera relationer explicit (Snyggt för VG)

        // Order -> OrderItems (En order har många rader)
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // SQL Server kräver att vi bestämmer precision för pengar (Decimal)
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(18,2)");
    }
}