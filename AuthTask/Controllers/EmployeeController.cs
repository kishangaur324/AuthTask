using AuthTask.Application.DTOs;
using AuthTask.Application.Interfaces;
using AuthTask.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthTask.Controllers
{
    /// <summary>
    /// Exposes employee management endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IValidator<UpdateEmployeeDto> _updateEmployeeValidator;
        private readonly IValidator<PaginationRequest> _paginationRequestValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeController"/> class.
        /// </summary>
        /// <param name="employeeService">Employee service.</param>
        /// <param name="updateEmployeeValidator">Update employee request validator.</param>
        /// <param name="paginationRequestValidator">Pagination request validator.</param>
        public EmployeeController(
            IEmployeeService employeeService,
            IValidator<UpdateEmployeeDto> updateEmployeeValidator,
            IValidator<PaginationRequest> paginationRequestValidator
        )
        {
            _employeeService = employeeService;
            _updateEmployeeValidator = updateEmployeeValidator;
            _paginationRequestValidator = paginationRequestValidator;
        }

        /// <summary>
        /// Updates an employee profile.
        /// </summary>
        /// <param name="updateEmployee">Updated employee data.</param>
        /// <returns>No content on success.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(
            typeof(ApiResponse<string>),
            StatusCodes.Status500InternalServerError
        )]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [Authorize]
        [Consumes("application/json")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateEmployeeDto updateEmployee)
        {
            var problem = await _updateEmployeeValidator.ValidateAsync<UpdateEmployeeDto>(updateEmployee);
            if (problem != null)
                return problem;

            var cancellationToken = HttpContext.RequestAborted;
            var result = await _employeeService.UpdateAsync(updateEmployee, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Gets a single employee by identifier.
        /// </summary>
        /// <param name="id">Employee identifier.</param>
        /// <returns>A standardized API response containing the employee data.</returns>
        [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(
            typeof(ApiResponse<EmployeeDto>),
            StatusCodes.Status500InternalServerError
        )]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid id)
        {
            var cancellationToken = HttpContext.RequestAborted;
            var result = await _employeeService.GetAsync(id, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Returns a paginated list of employees.
        /// </summary>
        /// <param name="request">Pagination and search criteria.</param>
        /// <returns>A standardized API response containing paginated employee data.</returns>
        [ProducesResponseType(
            typeof(ApiResponse<PaginationResponse<EmployeeDto>>),
            StatusCodes.Status200OK
        )]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
            typeof(ApiResponse<PaginationResponse<EmployeeDto>>),
            StatusCodes.Status500InternalServerError
        )]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [Authorize(Policy = "AdminOnly")]
        [Consumes("application/json")]
        [HttpPost("list")]
        public async Task<IActionResult> GetAllAsync([FromBody] PaginationRequest request)
        {
            var problem = await _paginationRequestValidator.ValidateAsync<PaginationRequest>(request);
            if (problem != null)
                return problem;

            var cancellationToken = HttpContext.RequestAborted;
            var result = await _employeeService.GetAllAsync(request, cancellationToken);
            return result.ToActionResult();
        }

        /// <summary>
        /// Deletes an employee by identifier.
        /// </summary>
        /// <param name="id">Employee identifier.</param>
        /// <returns>No content on success.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
            typeof(ApiResponse<string>),
            StatusCodes.Status500InternalServerError
        )]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var cancellationToken = HttpContext.RequestAborted;
            var result = await _employeeService.DeleteAsync(id, cancellationToken);
            return result.ToActionResult();
        }
    }
}
