using AuthTask.Application.Interfaces;
using AuthTask.Application.Services;
using AuthTask.Context;
using AuthTask.Filters;
using AuthTask.Infrastructure.Repositories;

namespace AuthTask.Extensions
{
    public static class RegisterDependenciesExtension
    {
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
