using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthTask.Extensions
{
    /// <summary>
    /// Configures authentication and authorization services.
    /// </summary>
    public static class RegisterAuthExtension
    {
        /// <summary>
        /// Registers JWT bearer authentication and authorization policies.
        /// </summary>
        /// <param name="services">Service collection to configure.</param>
        /// <param name="configuration">Application configuration.</param>
        /// <returns>The same service collection for chaining.</returns>
        public static IServiceCollection RegisterAuth(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var jwtConfig = configuration.GetSection("JwtConfig");

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "JwtBearer";
                    options.DefaultChallengeScheme = "JwtBearer";
                })
                .AddJwtBearer(
                    "JwtBearer",
                    options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = jwtConfig["Issuer"],
                            ValidAudience = jwtConfig["Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtConfig["SecretKey"]!)
                            ),
                        };
                    }
                );

            services
                .AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));

            return services;
        }
    }
}
