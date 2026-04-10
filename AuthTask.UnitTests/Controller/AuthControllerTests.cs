using AuthTask.Application.DTOs;
using AuthTask.Application.Interfaces;
using AuthTask.Controllers;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuthTask.UnitTests.Controller
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock = new();
        private readonly Mock<IValidator<RegisterRequest>> _registerValidatorMock = new();
        private readonly Mock<IValidator<LoginRequest>> _loginValidatorMock = new();

        // Happy path scenarios

        [Fact]
        public async Task RegisterAsync_ReturnsOk_WhenRequestIsValid()
        {
            var request = new RegisterRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
                Role = "employee",
            };
            var response = new RegisterResponse
            {
                EmployeeId = Guid.NewGuid(),
                Email = request.Email,
            };
            using var cts = new CancellationTokenSource();
            var controller = CreateController(cts.Token);

            _registerValidatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _authServiceMock
                .Setup(x => x.RegisterAsync(request, cts.Token))
                .ReturnsAsync(Result<RegisterResponse>.Success(response));

            var result = await controller.RegisterAsync(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<RegisterResponse>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(response.EmployeeId, apiResponse.Data.EmployeeId);
            Assert.Equal(response.Email, apiResponse.Data.Email);

            _authServiceMock.Verify(x => x.RegisterAsync(request, cts.Token), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ReturnsOk_WhenCredentialsAreValid()
        {
            var request = new LoginRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
            };
            var response = new LoginResponse { AccessToken = "token-value" };
            var controller = CreateController(CancellationToken.None);

            _loginValidatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _authServiceMock
                .Setup(x => x.LoginAsync(request))
                .ReturnsAsync(Result<LoginResponse>.Success(response));

            var result = await controller.LoginAsync(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<LoginResponse>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(response.AccessToken, apiResponse.Data.AccessToken);

            _authServiceMock.Verify(x => x.LoginAsync(request), Times.Once);
        }

        // Edge case scenarios

        [Fact]
        public async Task RegisterAsync_ReturnsBadRequest_WhenValidationFails()
        {
            var request = new RegisterRequest
            {
                Email = "kishan.singh@unthinkable",
                Password = "bad",
                Role = "employee",
            };
            var controller = CreateController(CancellationToken.None);
            var validationResult = new ValidationResult(
                [new ValidationFailure(nameof(RegisterRequest.Email), "Email is required")]
            );

            _registerValidatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var result = await controller.RegisterAsync(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<ValidationProblemDetails>(badRequest.Value);
            _authServiceMock.Verify(
                x => x.RegisterAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task RegisterAsync_ReturnsConflict_WhenServiceReturnsConflict()
        {
            var request = new RegisterRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
                Role = "employee",
            };
            var controller = CreateController(CancellationToken.None);

            _registerValidatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _authServiceMock
                .Setup(x => x.RegisterAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RegisterResponse>.Conflict("Duplicate user"));

            var result = await controller.RegisterAsync(request);

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(StatusCodes.Status409Conflict, conflict.StatusCode);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsInternalServerError_WhenServiceFails()
        {
            var request = new RegisterRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
                Role = "employee",
            };
            var controller = CreateController(CancellationToken.None);

            _registerValidatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _authServiceMock
                .Setup(x => x.RegisterAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RegisterResponse>.Failure("Registration failed"));

            var result = await controller.RegisterAsync(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [Fact]
        public async Task LoginAsync_ReturnsBadRequest_WhenValidationFails()
        {
            var request = new LoginRequest
            {
                Email = "kishan.singh@unthinkable",
                Password = "bad",
            };
            var controller = CreateController(CancellationToken.None);
            var validationResult = new ValidationResult(
                [new ValidationFailure(nameof(LoginRequest.Email), "Email is required")]
            );

            _loginValidatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var result = await controller.LoginAsync(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<ValidationProblemDetails>(badRequest.Value);
            _authServiceMock.Verify(x => x.LoginAsync(It.IsAny<LoginRequest>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ReturnsUnauthorized_WhenServiceReturnsUnauthorized()
        {
            var request = new LoginRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
            };
            var controller = CreateController(CancellationToken.None);

            _loginValidatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _authServiceMock
                .Setup(x => x.LoginAsync(request))
                .ReturnsAsync(Result<LoginResponse>.Unauthorized("Invalid credentials"));

            var result = await controller.LoginAsync(request);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(StatusCodes.Status401Unauthorized, unauthorized.StatusCode);
        }

        [Fact]
        public async Task LoginAsync_ReturnsInternalServerError_WhenServiceFails()
        {
            var request = new LoginRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
            };
            var controller = CreateController(CancellationToken.None);

            _loginValidatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _authServiceMock
                .Setup(x => x.LoginAsync(request))
                .ReturnsAsync(Result<LoginResponse>.Failure("Login failed"));

            var result = await controller.LoginAsync(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        private AuthController CreateController(CancellationToken requestAborted)
        {
            var controller = new AuthController(
                _authServiceMock.Object,
                _registerValidatorMock.Object,
                _loginValidatorMock.Object
            );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { RequestAborted = requestAborted },
            };

            return controller;
        }
    }
}
