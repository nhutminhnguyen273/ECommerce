using System.Linq.Expressions;
using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Application.Interfaces;
using ProductAPI.Domain.Entities;
using ProductAPI.Infrastructure.Data;

namespace ProductAPI.Infrastructure.Repositories
{
    public class ProductRepository(ProductDBContext context) : IProductRepository
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                // Check if the product already existed
                var getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
                if (getProduct != null && !string.IsNullOrEmpty(getProduct.Name))
                    return new Response(false, $"{entity.Name} already existed");

                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();

                if (currentEntity == null || currentEntity.Id <= 0)
                    return new Response(false, $"Error occurred while adding {entity.Name}");

                return new Response(true, $"{entity.Name} added to database successfully");
                
            } 
            catch (Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // Display scary-free message to the client
                return new Response(false, "Error occurred adding new product");
            } 
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product == null)
                    return new Response(false, $"{entity.Name} not found");
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} is deleted successfully");
            }
            catch (Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // Display scary-free message to the client
                return new Response(false, "Error occurred deleting new product");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product != null ? product : null!;
            }
            catch (Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // Display scary-free message to the client
                throw new Exception("Error occurred retrieving product");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync();
                return products != null ? products : null!;
            }
            catch (Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // Display scary-free message to the client
                throw new InvalidOperationException("Error occurred retrieving products");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync();
                return product != null ? product : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new InvalidOperationException("Error occurred retrieving product");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product == null)
                    return new Response(false, $"{entity.Name} not found");
                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} is updated successfully");
            }
            catch (Exception ex)
            {
                // Log the original exception
                LogException.LogExceptions(ex);

                // Display scary-free message to the client
                return new Response(false, "Error occurred updating new product");
            }
        }
    }
}
