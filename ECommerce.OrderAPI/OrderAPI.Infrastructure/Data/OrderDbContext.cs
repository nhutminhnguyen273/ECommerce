using Microsoft.EntityFrameworkCore;
using OrderAPI.Domain.Entities;

namespace OrderAPI.Infrastructure.Data
{
    public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }
    }
}
