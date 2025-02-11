using ECommerce.SharedLibrary.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductAPI.Application.Interfaces;
using ProductAPI.Infrastructure.Data;
using ProductAPI.Infrastructure.Repositories;

namespace ProductAPI.Infrastructure.DI
{
    public static class ServiceContainer
    {
        public static IServiceCollection  AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // Add database connectivity
            // Add authentication scheme
            SharedServiceContainer.AddSharedServices<ProductDBContext>(services, config, config["MySerilog:FileName"]!);

            // Create Dependency Injection (DI)
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Register middleware such as:
            // Global Exception: handles external error
            // Listen to Only API Gateway: block all outsider calls
            SharedServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}
