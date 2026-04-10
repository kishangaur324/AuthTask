using AuthTask.Application.Interfaces;
using AuthTask.Application.Services;
using AuthTask.Context;
using AuthTask.Filters;
using AuthTask.Infrastructure.Repositories;

namespace AuthTask.Extensions
{
    /// <summary>
    /// Registers application and infrastructure dependencies.
    /// </summary>
    public static class RegisterDependenciesExtension
    {
        /// <summary>
        /// Adds service registrations required by the application.
        /// </summary>
        /// <param name="services">Service collection to configure.</param>
        /// <returns>The same service collection for chaining.</returns>
        public static IServiceCollection RegisterDependencies(this IServiceCollection services)
        {
            services.AddScoped<ITrackingContextProvider, TrackingContextProvider>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeService, EmployeeService>();

            services.AddScoped<ApiKeyFilter>();
            return services;
        }
    }
}
