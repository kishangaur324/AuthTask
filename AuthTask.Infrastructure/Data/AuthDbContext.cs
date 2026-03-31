using AuthTask.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthTask.Infrastructure.Data
{
    public class AuthDbContext : IdentityDbContext<User>
    {
        public DbSet<Employee> Employees { get; set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options) { }
    }
}
