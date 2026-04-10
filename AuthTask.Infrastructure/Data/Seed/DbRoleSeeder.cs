using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AuthTask.Infrastructure.Data.Seed
{
    /// <summary>
    /// Seeds default application roles.
    /// </summary>
    public class DbRoleSeeder
    {
        /// <summary>
        /// Ensures required roles exist in the database.
        /// </summary>
        /// <param name="serviceProvider">Service provider used to resolve role manager.</param>
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = new string[] { "admin", "employee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
