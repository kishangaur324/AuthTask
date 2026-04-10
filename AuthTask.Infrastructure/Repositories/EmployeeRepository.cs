using AuthTask.Application.Interfaces;
using AuthTask.Domain.Entities;
using AuthTask.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthTask.Infrastructure.Repositories
{
    /// <summary>
    /// Entity Framework implementation of employee repository operations.
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AuthDbContext _authDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeRepository"/> class.
        /// </summary>
        /// <param name="authDbContext">Database context.</param>
        public EmployeeRepository(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        /// <inheritdoc />
        public async Task<(List<Employee>, int)> GetAllAsync(
            int skip,
            int take,
            string? search,
            CancellationToken cancellationToken
        )
        {
            // Restrict list endpoint to users assigned to the employee role.
            var query =
                from e in _authDbContext.Employees
                join ur in _authDbContext.UserRoles on e.UserId equals ur.UserId
                join r in _authDbContext.Roles on ur.RoleId equals r.Id
                where (search == null || e.Email.Contains(search)) && r.NormalizedName == "EMPLOYEE"
                select e;

            var totalCount = await query.Select(e => e.Id).Distinct().CountAsync(cancellationToken);
            var employees = await query
                .Distinct()
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
            return (employees, totalCount);
        }

        /// <inheritdoc />
        public async Task<Employee?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            return await _authDbContext.Employees.FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public async Task<Employee> AddAsync(
            Employee employee,
            CancellationToken cancellationToken = default
        )
        {
            await _authDbContext.Employees.AddAsync(employee, cancellationToken);

            return employee;
        }

        /// <inheritdoc />
        public async Task<Employee> UpdateAsync(Employee employee)
        {
            _authDbContext.Employees.Update(employee);
            return employee;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Guid id)
        {
            var employee = await _authDbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);
            _authDbContext.Employees.Remove(employee!);
        }
    }
}
