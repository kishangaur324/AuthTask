using AuthTask.Domain.Entities;

namespace AuthTask.Application.Interfaces
{
    /// <summary>
    /// Data-access abstraction for employee persistence operations.
    /// </summary>
    public interface IEmployeeRepository
    {
        /// <summary>
        /// Adds an employee entity to the store.
        /// </summary>
        /// <param name="employee">Employee entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The added employee entity.</returns>
        Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an employee by identifier.
        /// </summary>
        /// <param name="id">Employee identifier.</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Returns a paged collection of employee entities and total count.
        /// </summary>
        /// <param name="skip">Number of records to skip.</param>
        /// <param name="take">Number of records to take.</param>
        /// <param name="search">Optional email search term.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Employee page and total distinct count.</returns>
        Task<(List<Employee>, int)> GetAllAsync(
            int skip,
            int take,
            string? search,
            CancellationToken cancellationToken
        );

        /// <summary>
        /// Gets an employee by identifier.
        /// </summary>
        /// <param name="id">Employee identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Employee entity or <see langword="null"/> when not found.</returns>
        Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing employee entity.
        /// </summary>
        /// <param name="employee">Employee entity.</param>
        /// <returns>The updated employee entity.</returns>
        Task<Employee> UpdateAsync(Employee employee);
    }
}
