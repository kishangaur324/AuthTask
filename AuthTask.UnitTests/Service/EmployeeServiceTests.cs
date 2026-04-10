using System.Security.Claims;
using AuthTask.Application.DTOs;
using AuthTask.Application.Interfaces;
using AuthTask.Application.Services;
using AuthTask.Domain.Entities;
using AuthTask.MapperProfiles;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AuthTask.UnitTests.Service
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<EmployeeService>> _loggerMock = new();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
        private readonly Mock<IValidator<CreateEmployeeDto>> _createEmployeeValidatorMock = new();
        private readonly IMapper _mapper = CreateMapper();

        // Happy path scenarios

        [Fact]
        public async Task AddAsync_ReturnsEmployeeId_WhenRequestIsValid()
        {
            var request = CreateValidCreateRequest();
            var expectedEmployeeId = Guid.NewGuid();
            using var cts = new CancellationTokenSource();
            Employee? capturedEmployee = null;

            _createEmployeeValidatorMock
                .Setup(x => x.ValidateAsync(request, cts.Token))
                .ReturnsAsync(new ValidationResult());
            _employeeRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Employee>(), cts.Token))
                .Callback<Employee, CancellationToken>((employee, _) => capturedEmployee = employee)
                .ReturnsAsync(
                    (Employee employee, CancellationToken _) =>
                    {
                        employee.Id = expectedEmployeeId;
                        return employee;
                    }
                );

            var result = await CreateService().AddAsync(request, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Success, result.Status);
            Assert.Equal(expectedEmployeeId, result.Data);
            Assert.NotNull(capturedEmployee);
            Assert.Equal(request.Email, capturedEmployee!.Email);
            Assert.Equal(request.UserId, capturedEmployee.UserId);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNoContent_WhenEmployeeExists()
        {
            var request = CreateValidUpdateRequest();
            var existingEmployee = CreateEmployee(request.Id, "legacy-user");
            using var cts = new CancellationTokenSource();

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(request.Id, cts.Token))
                .ReturnsAsync(existingEmployee);
            _employeeRepositoryMock
                .Setup(x => x.UpdateAsync(existingEmployee))
                .ReturnsAsync(existingEmployee);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(cts.Token)).ReturnsAsync(1);

            var result = await CreateService().UpdateAsync(request, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.NoContent, result.Status);
            Assert.Equal(request.FirstName, existingEmployee.FirstName);
            Assert.Equal(request.LastName, existingEmployee.LastName);
            Assert.Equal(request.Email, existingEmployee.Email);
            Assert.Equal(request.PhoneNumber, existingEmployee.PhoneNumber);
            Assert.Equal(request.EmployeeCode, existingEmployee.EmployeeCode);
            Assert.Equal(request.IsActive, existingEmployee.IsActive);
            Assert.Equal(request.DepartmentId, existingEmployee.DepartmentId);
            Assert.Equal(request.ManagerId, existingEmployee.ManagerId);
            Assert.Equal(request.DateOfJoining, existingEmployee.DateOfJoining);
            Assert.Equal(request.DateOfLeaving, existingEmployee.DateOfLeaving);
            _employeeRepositoryMock.Verify(x => x.UpdateAsync(existingEmployee), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(cts.Token), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenEmployeeExists()
        {
            var employeeId = Guid.NewGuid();
            using var cts = new CancellationTokenSource();

            _employeeRepositoryMock
                .Setup(x => x.DeleteAsync(employeeId))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(cts.Token)).ReturnsAsync(1);

            var result = await CreateService().DeleteAsync(employeeId, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.NoContent, result.Status);
            _employeeRepositoryMock.Verify(x => x.DeleteAsync(employeeId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(cts.Token), Times.Once);
        }

        [Fact]
        public async Task GetAsync_ReturnsEmployee_WhenRequestingUserIsAdmin()
        {
            var employeeId = Guid.NewGuid();
            var employee = CreateEmployee(employeeId, "target-user");
            using var cts = new CancellationTokenSource();

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId, cts.Token))
                .ReturnsAsync(employee);

            var result = await CreateService(CreatePrincipal("admin-user", isAdmin: true))
                .GetAsync(employeeId, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Success, result.Status);
            Assert.NotNull(result.Data);
            Assert.Equal(employee.Id, result.Data.Id);
        }

        [Fact]
        public async Task GetAsync_ReturnsEmployee_WhenRequestingUserOwnsTheEmployee()
        {
            var employeeId = Guid.NewGuid();
            var ownerId = "owner-user";
            var employee = CreateEmployee(employeeId, ownerId);
            using var cts = new CancellationTokenSource();

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId, cts.Token))
                .ReturnsAsync(employee);

            var result = await CreateService(CreatePrincipal(ownerId, isAdmin: false))
                .GetAsync(employeeId, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Success, result.Status);
            Assert.NotNull(result.Data);
            Assert.Equal(employee.Id, result.Data.Id);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmployees_WhenRequestIsValid()
        {
            var request = new PaginationRequest
            {
                PageNumber = 1,
                PageSize = 2,
                Search = "example",
            };
            var employees = new List<Employee>
            {
                CreateEmployee(Guid.NewGuid(), "user-1"),
                CreateEmployee(Guid.NewGuid(), "user-2"),
            };
            using var cts = new CancellationTokenSource();

            _employeeRepositoryMock
                .Setup(x =>
                    x.GetAllAsync(request.Skip, request.PageSize, request.Search, cts.Token)
                )
                .ReturnsAsync((employees, 2));

            var result = await CreateService().GetAllAsync(request, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Success, result.Status);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.TotalCount);
            Assert.Equal(2, result.Data.Items.Count);
            Assert.All(result.Data.Items, item => Assert.Contains("@unthinkable.co", item.Email));
        }

        // Edge case scenarios

        [Fact]
        public async Task AddAsync_ReturnsFailure_WhenValidationFails()
        {
            var request = CreateValidCreateRequest();
            using var cts = new CancellationTokenSource();
            var validationResult = new ValidationResult(
                new[]
                {
                    new ValidationFailure(nameof(CreateEmployeeDto.Email), "Email is required"),
                    new ValidationFailure(
                        nameof(CreateEmployeeDto.DateOfJoining),
                        "Date of joining is required"
                    ),
                }
            );

            _createEmployeeValidatorMock
                .Setup(x => x.ValidateAsync(request, cts.Token))
                .ReturnsAsync(validationResult);

            var result = await CreateService().AddAsync(request, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Failure, result.Status);
            Assert.Contains("Email is required", result.Error);
            Assert.Contains("Date of joining is required", result.Error);
            _employeeRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task AddAsync_Throws_WhenRepositoryThrows()
        {
            var request = CreateValidCreateRequest();
            using var cts = new CancellationTokenSource();

            _createEmployeeValidatorMock
                .Setup(x => x.ValidateAsync(request, cts.Token))
                .ReturnsAsync(new ValidationResult());
            _employeeRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Employee>(), cts.Token))
                .ThrowsAsync(new InvalidOperationException("Insert failed"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                CreateService().AddAsync(request, cts.Token)
            );
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            var request = CreateValidUpdateRequest();
            using var cts = new CancellationTokenSource();

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(request.Id, cts.Token))
                .ReturnsAsync((Employee?)null);

            var result = await CreateService().UpdateAsync(request, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.NotFound, result.Status);
            Assert.Equal("Employee not found", result.Error);
            _employeeRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Employee>()), Times.Never);
            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFailure_WhenRepositoryThrows()
        {
            var request = CreateValidUpdateRequest();
            using var cts = new CancellationTokenSource();

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(request.Id, cts.Token))
                .ThrowsAsync(new InvalidOperationException("Load failed"));

            var result = await CreateService().UpdateAsync(request, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Failure, result.Status);
            Assert.Equal("Failed to update the employee data.", result.Error);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFailure_WhenRepositoryThrows()
        {
            var employeeId = Guid.NewGuid();
            using var cts = new CancellationTokenSource();

            _employeeRepositoryMock
                .Setup(x => x.DeleteAsync(employeeId))
                .ThrowsAsync(new InvalidOperationException("Delete failed"));

            var result = await CreateService().DeleteAsync(employeeId, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Failure, result.Status);
            Assert.Equal("Failed to delete the employee data.", result.Error);
            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task GetAsync_ReturnsUnauthorized_WhenUserIsNotOwnerAndNotAdmin()
        {
            var employeeId = Guid.NewGuid();
            var employee = CreateEmployee(employeeId, "owner-user");
            using var cts = new CancellationTokenSource();

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId, cts.Token))
                .ReturnsAsync(employee);

            var result = await CreateService(CreatePrincipal("different-user", isAdmin: false))
                .GetAsync(employeeId, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Unauthorized, result.Status);
            Assert.Equal("User is unauthorized.", result.Error);
        }

        [Fact]
        public async Task GetAsync_ReturnsNotFound_WhenEmployeeDoesNotExist()
        {
            var employeeId = Guid.NewGuid();
            using var cts = new CancellationTokenSource();

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId, cts.Token))
                .ReturnsAsync((Employee?)null);

            var result = await CreateService(CreatePrincipal("user-1", isAdmin: false))
                .GetAsync(employeeId, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.NotFound, result.Status);
            Assert.Equal("User not found.", result.Error);
        }

        [Fact]
        public async Task GetAsync_ReturnsFailure_WhenHttpContextIsMissing()
        {
            var employeeId = Guid.NewGuid();
            var employee = CreateEmployee(employeeId, "owner-user");
            using var cts = new CancellationTokenSource();

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId, cts.Token))
                .ReturnsAsync(employee);

            var result = await CreateService(null, includeHttpContext: false)
                .GetAsync(employeeId, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Failure, result.Status);
            Assert.Equal("Failed to fetch the employee data.", result.Error);
        }

        [Fact]
        public async Task GetAsync_ReturnsFailure_WhenRepositoryThrows()
        {
            var employeeId = Guid.NewGuid();
            using var cts = new CancellationTokenSource();

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId, cts.Token))
                .ThrowsAsync(new InvalidOperationException("Read failed"));

            var result = await CreateService(CreatePrincipal("user-1", isAdmin: false))
                .GetAsync(employeeId, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Failure, result.Status);
            Assert.Equal("Failed to fetch the employee data.", result.Error);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsFailure_WhenRepositoryThrows()
        {
            var request = new PaginationRequest { PageNumber = 1, PageSize = 10 };
            using var cts = new CancellationTokenSource();

            _employeeRepositoryMock
                .Setup(x =>
                    x.GetAllAsync(request.Skip, request.PageSize, request.Search, cts.Token)
                )
                .ThrowsAsync(new InvalidOperationException("List failed"));

            var result = await CreateService().GetAllAsync(request, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Failure, result.Status);
            Assert.Equal("Failed to fetch the employees data.", result.Error);
        }

        private EmployeeService CreateService(
            ClaimsPrincipal? principal = null,
            bool includeHttpContext = true
        )
        {
            if (includeHttpContext)
            {
                var httpContext = new DefaultHttpContext
                {
                    User = principal ?? new ClaimsPrincipal(new ClaimsIdentity()),
                };
                _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            }
            else
            {
                _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
            }

            return new EmployeeService(
                _employeeRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _mapper,
                _loggerMock.Object,
                _httpContextAccessorMock.Object,
                _createEmployeeValidatorMock.Object
            );
        }

        private static IMapper CreateMapper()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddProfile<MapperProfile>();
                },
                NullLoggerFactory.Instance
            );
            return mapperConfiguration.CreateMapper();
        }

        private static ClaimsPrincipal CreatePrincipal(string? userId, bool isAdmin)
        {
            var claims = new List<Claim>();

            if (!string.IsNullOrWhiteSpace(userId))
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

            if (isAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "admin"));

            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        private static CreateEmployeeDto CreateValidCreateRequest()
        {
            return new CreateEmployeeDto
            {
                UserId = "user-1",
                Email = "kishan.singh@unthinkable.co",
                EmployeeCode = "EMP-001",
                FirstName = "Kishan",
                LastName = "Singh",
                PhoneNumber = "+919999999999",
                DateOfJoining = DateTime.UtcNow.AddDays(-30),
            };
        }

        private static UpdateEmployeeDto CreateValidUpdateRequest()
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
                DateOfJoining = DateTime.UtcNow.AddDays(-30),
                DateOfLeaving = DateTime.UtcNow.AddDays(30),
                IsActive = true,
            };
        }

        private static Employee CreateEmployee(Guid id, string userId)
        {
            return new Employee
            {
                Id = id,
                UserId = userId,
                EmployeeCode = "EMP-001",
                FirstName = "Kishan",
                LastName = "Singh",
                Email = "kishan.singh@unthinkable.co",
                PhoneNumber = "+919999999999",
                DepartmentId = Guid.NewGuid(),
                ManagerId = Guid.NewGuid(),
                DateOfJoining = DateTime.UtcNow.AddDays(-30),
                DateOfLeaving = null,
                IsActive = true,
            };
        }
    }
}
