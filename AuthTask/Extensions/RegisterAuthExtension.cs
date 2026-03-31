using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthTask.Extensions
{
    public static class RegisterAuthExtension
    {
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
