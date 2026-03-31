using AuthTask.Application.DTOs;
using AuthTask.Application.Interfaces;
using AuthTask.Extensions;
using AuthTask.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AuthTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(
            typeof(ApiResponse<RegisterResponse>),
            StatusCodes.Status500InternalServerError
        )]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ApiKey]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [Consumes("application/x-www-form-urlencoded", "multipart/form-data")]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromForm] RegisterRequest request)
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            var result = await _authService.RegisterAsync(request, cancellationToken);
            return result.ToActionResult();
        }

        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
            typeof(ApiResponse<LoginResponse>),
            StatusCodes.Status500InternalServerError
        )]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ApiKey]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [Consumes("application/x-www-form-urlencoded", "multipart/form-data")]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromForm] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return result.ToActionResult();
        }
    }
}
