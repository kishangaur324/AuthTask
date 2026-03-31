using AuthTask.Application.DTOs;
using AuthTask.Application.Interfaces;
using AuthTask.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

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
            var cancellationToken = HttpContext.RequestAborted;
            var result = await _employeeService.UpdateAsync(updateEmployee, cancellationToken);
            return result.ToActionResult();
        }

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
            var cancellationToken = HttpContext.RequestAborted;
            var result = await _employeeService.GetAllAsync(request, cancellationToken);
            return result.ToActionResult();
        }

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
