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
    public class EmployeeControllerTests
    {
        private readonly Mock<IEmployeeService> _employeeServiceMock = new();
        private readonly Mock<IValidator<UpdateEmployeeDto>> _updateValidatorMock = new();
        private readonly Mock<IValidator<PaginationRequest>> _paginationValidatorMock = new();

        // Happy path scenarios

        [Fact]
        public async Task UpdateAsync_ReturnsNoContent_WhenRequestIsValid()
        {
            var request = CreateUpdateRequest();
            using var cts = new CancellationTokenSource();
            var controller = CreateController(cts.Token);

            _updateValidatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _employeeServiceMock
                .Setup(x => x.UpdateAsync(request, cts.Token))
                .ReturnsAsync(Result<string>.NoContent());

            var result = await controller.UpdateAsync(request);

            Assert.IsType<NoContentResult>(result);
            _employeeServiceMock.Verify(x => x.UpdateAsync(request, cts.Token), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsOk_WhenEmployeeExists()
        {
            var employeeId = Guid.NewGuid();
            using var cts = new CancellationTokenSource();
            var controller = CreateController(cts.Token);
            var dto = CreateEmployeeDto(employeeId);

            _employeeServiceMock
                .Setup(x => x.GetAsync(employeeId, cts.Token))
                .ReturnsAsync(Result<EmployeeDto>.Success(dto));

            var result = await controller.GetAsync(employeeId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<EmployeeDto>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(employeeId, apiResponse.Data.Id);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOk_WhenRequestIsValid()
        {
            var request = new PaginationRequest { PageNumber = 1, PageSize = 10 };
            using var cts = new CancellationTokenSource();
            var controller = CreateController(cts.Token);
            var response = new PaginationResponse<EmployeeDto>
            {
                TotalCount = 1,
                Items = [CreateEmployeeDto(Guid.NewGuid())],
            };

            _paginationValidatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _employeeServiceMock
                .Setup(x => x.GetAllAsync(request, cts.Token))
                .ReturnsAsync(Result<PaginationResponse<EmployeeDto>>.Success(response));

            var result = await controller.GetAllAsync(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<PaginationResponse<EmployeeDto>>>(
                okResult.Value
            );
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(1, apiResponse.Data.TotalCount);
            Assert.Single(apiResponse.Data.Items);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenEmployeeExists()
        {
            var employeeId = Guid.NewGuid();
            using var cts = new CancellationTokenSource();
            var controller = CreateController(cts.Token);

            _employeeServiceMock
                .Setup(x => x.DeleteAsync(employeeId, cts.Token))
                .ReturnsAsync(Result<string>.NoContent());

            var result = await controller.DeleteAsync(employeeId);

            Assert.IsType<NoContentResult>(result);
            _employeeServiceMock.Verify(x => x.DeleteAsync(employeeId, cts.Token), Times.Once);
        }

        // Edge case scenarios

        [Fact]
        public async Task UpdateAsync_ReturnsBadRequest_WhenValidationFails()
        {
            var request = CreateUpdateRequest();
            var controller = CreateController(CancellationToken.None);
            var validationResult = new ValidationResult([
                new ValidationFailure(nameof(UpdateEmployeeDto.Email), "Email is required"),
            ]);

            _updateValidatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var result = await controller.UpdateAsync(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<ValidationProblemDetails>(badRequest.Value);
            _employeeServiceMock.Verify(
                x => x.UpdateAsync(It.IsAny<UpdateEmployeeDto>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenServiceReturnsNotFound()
        {
            var request = CreateUpdateRequest();
            var controller = CreateController(CancellationToken.None);

            _updateValidatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _employeeServiceMock
                .Setup(x => x.UpdateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<string>.NotFound("Employee not found"));

            var result = await controller.UpdateAsync(request);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
        }

        [Fact]
        public async Task GetAsync_ReturnsUnauthorized_WhenServiceReturnsUnauthorized()
        {
            var employeeId = Guid.NewGuid();
            var controller = CreateController(CancellationToken.None);

            _employeeServiceMock
                .Setup(x => x.GetAsync(employeeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmployeeDto>.Unauthorized("User is unauthorized."));

            var result = await controller.GetAsync(employeeId);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(StatusCodes.Status401Unauthorized, unauthorized.StatusCode);
        }

        [Fact]
        public async Task GetAsync_ReturnsNotFound_WhenServiceReturnsNotFound()
        {
            var employeeId = Guid.NewGuid();
            var controller = CreateController(CancellationToken.None);

            _employeeServiceMock
                .Setup(x => x.GetAsync(employeeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<EmployeeDto>.NotFound("User not found."));

            var result = await controller.GetAsync(employeeId);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFound.StatusCode);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsBadRequest_WhenValidationFails()
        {
            var request = new PaginationRequest { PageNumber = 0, PageSize = 200 };
            var controller = CreateController(CancellationToken.None);
            var validationResult = new ValidationResult([
                new ValidationFailure(nameof(PaginationRequest.PageSize), "Page size invalid"),
            ]);

            _paginationValidatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var result = await controller.GetAllAsync(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<ValidationProblemDetails>(badRequest.Value);
            _employeeServiceMock.Verify(
                x => x.GetAllAsync(It.IsAny<PaginationRequest>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task GetAllAsync_ReturnsInternalServerError_WhenServiceFails()
        {
            var request = new PaginationRequest { PageNumber = 1, PageSize = 10 };
            var controller = CreateController(CancellationToken.None);

            _paginationValidatorMock
                .Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            _employeeServiceMock
                .Setup(x => x.GetAllAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    Result<PaginationResponse<EmployeeDto>>.Failure("Failed to fetch data.")
                );

            var result = await controller.GetAllAsync(request);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsInternalServerError_WhenServiceFails()
        {
            var employeeId = Guid.NewGuid();
            var controller = CreateController(CancellationToken.None);

            _employeeServiceMock
                .Setup(x => x.DeleteAsync(employeeId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<string>.Failure("Failed to delete employee."));

            var result = await controller.DeleteAsync(employeeId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        private EmployeeController CreateController(CancellationToken requestAborted)
        {
            var controller = new EmployeeController(
                _employeeServiceMock.Object,
                _updateValidatorMock.Object,
                _paginationValidatorMock.Object
            );
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { RequestAborted = requestAborted },
            };

            return controller;
        }

        private static UpdateEmployeeDto CreateUpdateRequest()
        {
            return new UpdateEmployeeDto
            {
                Id = Guid.NewGuid(),
                EmployeeCode = "EMP-001",
                FirstName = "Kishan",
                LastName = "Singh",
                Email = "kishan.singh@unthinkable.co",
                PhoneNumber = "+919999999999",
                DepartmentId = Guid.NewGuid(),
                ManagerId = Guid.NewGuid(),
                DateOfJoining = DateTime.UtcNow.AddDays(-7),
                DateOfLeaving = null,
                IsActive = true,
            };
        }

        private static EmployeeDto CreateEmployeeDto(Guid id)
        {
            return new EmployeeDto
            {
                Id = id,
                EmployeeCode = "EMP-001",
                FirstName = "Kishan",
                LastName = "Singh",
                Email = "kishan.singh@unthinkable.co",
                PhoneNumber = "+919999999999",
                DateOfJoining = DateTime.UtcNow.AddMonths(-2),
                DateOfLeaving = null,
                IsActive = true,
            };
        }
    }
}
