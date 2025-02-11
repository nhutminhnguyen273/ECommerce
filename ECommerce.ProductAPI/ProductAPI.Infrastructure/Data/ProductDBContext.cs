using Microsoft.EntityFrameworkCore;
using ProductAPI.Domain.Entities;

namespace ProductAPI.Infrastructure.Data
{
    public class ProductDBContext(DbContextOptions<ProductDBContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
    }
}
