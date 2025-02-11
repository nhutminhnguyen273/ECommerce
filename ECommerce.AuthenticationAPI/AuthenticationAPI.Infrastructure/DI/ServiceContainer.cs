using AuthenticationAPI.Application.Interfaces;
using AuthenticationAPI.Infrastructure.Data;
using AuthenticationAPI.Infrastructure.Repositories;
using ECommerce.SharedLibrary.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationAPI.Infrastructure.DI
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // Add database connectivity
            // Add Authentication Scheme
            SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(services, config, config["MySerilog:FileName"]!);

            // Create Dependency Injection
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        public static IApplicationBuilder UserInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Register middleware such as:
            // Global Exception: Handle external errors.
            // Listen Only to API Gateway: block all outsiders call.
            SharedServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}
