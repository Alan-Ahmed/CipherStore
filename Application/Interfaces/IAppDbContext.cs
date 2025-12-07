using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Application.Interfaces;

public interface IAppDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<AdminUser> AdminUsers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}