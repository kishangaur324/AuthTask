using AuthTask.Application.DTOs;

namespace AuthTask.Application.Interfaces
{
    /// <summary>
    /// Provides employee-related business operations.
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Creates a new employee record.
        /// </summary>
        /// <param name="createEmployee">Employee data to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Operation result containing the created employee identifier.</returns>
        Task<Result<Guid>> AddAsync(
            CreateEmployeeDto createEmployee,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Deletes an employee by identifier.
        /// </summary>
        /// <param name="id">Employee identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Operation result for delete execution.</returns>
        Task<Result<string>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paginated list of employees.
        /// </summary>
        /// <param name="pagination">Pagination and search criteria.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Operation result containing paginated employee data.</returns>
        Task<Result<PaginationResponse<EmployeeDto>>> GetAllAsync(
            PaginationRequest pagination,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Retrieves an employee by identifier.
        /// </summary>
        /// <param name="id">Employee identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Operation result containing employee data.</returns>
        Task<Result<EmployeeDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing employee.
        /// </summary>
        /// <param name="updateEmployee">Updated employee data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Operation result for update execution.</returns>
        Task<Result<string>> UpdateAsync(
            UpdateEmployeeDto updateEmployee,
            CancellationToken cancellationToken = default
        );
    }
}
