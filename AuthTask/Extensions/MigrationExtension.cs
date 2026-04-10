using AuthTask.Infrastructure.Data;
using AuthTask.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;

namespace AuthTask.Extensions
{
    /// <summary>
    /// Database initialization helpers for migrations and seed data.
    /// </summary>
    public static class MigrationExtension
    {
        /// <summary>
        /// Applies pending migrations and seeds role data with retry support.
        /// </summary>
        /// <param name="app">Web application instance.</param>
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            const int MaxRetries = 3;
            var migrationRetry = 0;
            while (migrationRetry < MaxRetries)
            {
                try
                {
                    // Retry to tolerate transient startup/database readiness issues.
                    await db.Database.MigrateAsync();
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError("Failed to run DB migrations. {ex}", ex);
                    migrationRetry++;
                }
            }

            var seedRetry = 0;
            while (seedRetry < MaxRetries)
            {
                try
                {
                    // Roles are seeded after migration so the schema is guaranteed to exist.
                    await DbRoleSeeder.SeedRolesAsync(scope.ServiceProvider);
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError("Failed to seed roles data. {ex}", ex);
                    seedRetry++;
                }
            }
        }
    }
}
