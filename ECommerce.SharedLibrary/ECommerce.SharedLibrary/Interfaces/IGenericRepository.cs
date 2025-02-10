using System.Linq.Expressions;
using ECommerce.SharedLibrary.Responses;

namespace ECommerce.SharedLibrary.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<Response> CreateAsync(T entity);
        Task<Response> UpdateAsync(T entity);
        Task<Response> DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> FindByIdAsync(int id);
        Task<T> GetByAsync(Expression<Func<T, bool>> predicate);
    }
}
