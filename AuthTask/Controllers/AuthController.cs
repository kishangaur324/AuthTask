using AuthTask.Application.DTOs;
using AuthTask.Application.Interfaces;
using AuthTask.Extensions;
using AuthTask.Filters;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AuthTask.Controllers
{
    /// <summary>
    /// Exposes authentication endpoints for user registration and login.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">Authentication service.</param>
        /// <param name="registerValidator">RegisterRequest validator.</param>
        /// <param name="loginValidator">LoginRequest validator.</param>
        public AuthController(
            IAuthService authService,
            IValidator<RegisterRequest> registerValidator,
            IValidator<LoginRequest> loginValidator
        )
        {
            _authService = authService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        /// <summary>
        /// Registers a new user and associated employee record.
        /// </summary>
        /// <param name="request">Registration payload submitted as form data.</param>
        /// <returns>A standardized API response containing registration details.</returns>
        [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(
            typeof(ApiResponse<string>),
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
            var problem = await _registerValidator.ValidateAsync<RegisterRequest>(request);
            if (problem != null)
                return problem;

            var cancellationToken = HttpContext.RequestAborted;

            var result = await _authService.RegisterAsync(request, cancellationToken);

            return result.ToActionResult();
        }

        /// <summary>
        /// Authenticates a user and returns a JWT access token.
        /// </summary>
        /// <param name="request">Login payload submitted as form data.</param>
        /// <returns>A standardized API response containing the generated access token.</returns>
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(
            typeof(ApiResponse<string>),
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
            var problem = await _loginValidator.ValidateAsync<LoginRequest>(request);
            if (problem != null)
                return problem;

            var result = await _authService.LoginAsync(request);
            return result.ToActionResult();
        }
    }
}
