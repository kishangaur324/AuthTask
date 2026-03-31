using AuthTask.Application.Interfaces;
using AuthTask.Domain.Entities;
using AuthTask.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthTask.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AuthDbContext _authDbContext;

        public EmployeeRepository(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }

        public async Task<(List<Employee>, int)> GetAllAsync(
            int skip,
            int take,
            string? search,
            CancellationToken cancellationToken
        )
        {
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

        public async Task<Employee> AddAsync(
            Employee employee,
            CancellationToken cancellationToken = default
        )
        {
            await _authDbContext.Employees.AddAsync(employee, cancellationToken);

            return employee;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            _authDbContext.Employees.Update(employee);
            return employee;
        }

        public async Task DeleteAsync(Guid id)
        {
            var employee = await _authDbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);
            _authDbContext.Employees.Remove(employee!);
        }
    }
}
