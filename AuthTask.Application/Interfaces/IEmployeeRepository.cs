using AuthTask.Domain.Entities;

namespace AuthTask.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id);
        Task<(List<Employee>, int)> GetAllAsync(
            int skip,
            int take,
            string? search,
            CancellationToken cancellationToken
        );
        Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Employee> UpdateAsync(Employee employee);
    }
}
