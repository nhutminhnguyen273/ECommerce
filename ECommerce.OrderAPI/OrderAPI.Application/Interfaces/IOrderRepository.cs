using System.Linq.Expressions;
using ECommerce.SharedLibrary.Interfaces;
using OrderAPI.Domain.Entities;

namespace OrderAPI.Application.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate);
    }
}
