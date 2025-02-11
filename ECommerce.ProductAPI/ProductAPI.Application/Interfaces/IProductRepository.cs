using ECommerce.SharedLibrary.Interfaces;
using ProductAPI.Domain.Entities;

namespace ProductAPI.Application.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
    }
}
