using AuthTask.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthTask.Infrastructure.Data
{
    /// <summary>
    /// Entity Framework database context for identity and employee tables.
    /// </summary>
    public class AuthDbContext : IdentityDbContext<User>
    {
        /// <summary>
        /// Gets or sets employee entities.
        /// </summary>
        public DbSet<Employee> Employees { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthDbContext"/> class.
        /// </summary>
        /// <param name="options">Database context options.</param>
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options) { }
    }
}
