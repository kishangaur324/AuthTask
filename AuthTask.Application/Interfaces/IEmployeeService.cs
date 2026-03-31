using AuthTask.Application.DTOs;

namespace AuthTask.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<Result<Guid>> AddAsync(
            CreateEmployeeDto createEmployee,
            CancellationToken cancellationToken = default
        );
        Task<Result<string>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<PaginationResponse<EmployeeDto>>> GetAllAsync(
            PaginationRequest pagination,
            CancellationToken cancellationToken = default
        );
        Task<Result<EmployeeDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<string>> UpdateAsync(
            UpdateEmployeeDto updateEmployee,
            CancellationToken cancellationToken = default
        );
    }
}
