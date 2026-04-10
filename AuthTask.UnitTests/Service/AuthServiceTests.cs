using AuthTask.Application.DTOs;
using AuthTask.Application.Interfaces;
using AuthTask.Application.Services;
using AuthTask.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace AuthTask.UnitTests.Service
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock = CreateUserManagerMock();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<ILogger<AuthService>> _loggerMock = new();
        private readonly Mock<IEmployeeService> _employeeServiceMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        // Happy path scenarios

        [Fact]
        public async Task RegisterAsync_ReturnsSuccess_WhenRequestIsValid()
        {
            var request = new RegisterRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
                Role = "employee",
            };
            var employeeId = Guid.NewGuid();
            using var cts = new CancellationTokenSource();
            CreateService();

            _unitOfWorkMock
                .Setup(x => x.BeginTransactionAsync(cts.Token))
                .Returns(Task.CompletedTask);
            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
                .Callback<User, string>((user, _) => user.Id = "user-1")
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock
                .Setup(x => x.AddToRoleAsync(It.IsAny<User>(), request.Role))
                .ReturnsAsync(IdentityResult.Success);
            _employeeServiceMock
                .Setup(x =>
                    x.AddAsync(
                        It.Is<CreateEmployeeDto>(dto =>
                            dto.UserId == "user-1" && dto.Email == request.Email
                        ),
                        cts.Token
                    )
                )
                .ReturnsAsync(Result<Guid>.Success(employeeId));
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(cts.Token)).ReturnsAsync(1);
            _unitOfWorkMock.Setup(x => x.CommitAsync(cts.Token)).Returns(Task.CompletedTask);

            var result = await CreateService().RegisterAsync(request, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Success, result.Status);
            Assert.NotNull(result.Data);
            Assert.Equal(employeeId, result.Data.EmployeeId);
            Assert.Equal(request.Email, result.Data.Email);
            _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(cts.Token), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(cts.Token), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(cts.Token), Times.Once);
            _unitOfWorkMock.Verify(
                x => x.RollbackAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
        {
            var request = new LoginRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
            };
            var user = new User
            {
                Id = "user-1",
                Email = request.Email,
                UserName = request.Email,
            };
            var roles = new List<string> { "employee" };
            const string token = "generated-token";

            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            _userManagerMock
                .Setup(x => x.CheckPasswordAsync(user, request.Password))
                .ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
            _tokenServiceMock.Setup(x => x.GenerateToken(user, roles)).Returns(token);

            var result = await CreateService().LoginAsync(request);

            Assert.Equal(Application.Enums.ResultStatus.Success, result.Status);
            Assert.NotNull(result.Data);
            Assert.Equal(token, result.Data.AccessToken);
            _tokenServiceMock.Verify(x => x.GenerateToken(user, roles), Times.Once);
        }

        // Edge case scenarios

        [Fact]
        public async Task RegisterAsync_ReturnsConflict_WhenCreateFailsWithDuplicateUserName()
        {
            var request = new RegisterRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
                Role = "employee",
            };
            using var cts = new CancellationTokenSource();
            var duplicateError = IdentityResult.Failed(
                new IdentityError { Code = "DuplicateUserName", Description = "Duplicate username" }
            );

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(duplicateError);

            var result = await CreateService().RegisterAsync(request, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Conflict, result.Status);
            Assert.Contains("Duplicate username", result.Error);
            _userManagerMock.Verify(
                x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()),
                Times.Never
            );
            _employeeServiceMock.Verify(
                x => x.AddAsync(It.IsAny<CreateEmployeeDto>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
            _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsFailure_WhenCreateFailsWithNonDuplicateError()
        {
            var request = new RegisterRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
                Role = "employee",
            };
            using var cts = new CancellationTokenSource();
            var identityError = IdentityResult.Failed(
                new IdentityError
                {
                    Code = "PasswordTooShort",
                    Description = "Password is too short",
                }
            );

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(identityError);

            var result = await CreateService().RegisterAsync(request, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Failure, result.Status);
            Assert.Contains("Password is too short", result.Error);
            _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_RollsBackAndReturnsFailure_WhenAddRoleFails()
        {
            var request = new RegisterRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
                Role = "employee",
            };
            using var cts = new CancellationTokenSource();
            var identityError = IdentityResult.Failed(
                new IdentityError { Code = "InvalidRole", Description = "Role not found" }
            );

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
                .Callback<User, string>((user, _) => user.Id = "user-1")
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock
                .Setup(x => x.AddToRoleAsync(It.IsAny<User>(), request.Role))
                .ReturnsAsync(identityError);

            var result = await CreateService().RegisterAsync(request, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Failure, result.Status);
            Assert.Contains("Role not found", result.Error);
            _unitOfWorkMock.Verify(x => x.RollbackAsync(cts.Token), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            _employeeServiceMock.Verify(
                x => x.AddAsync(It.IsAny<CreateEmployeeDto>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task RegisterAsync_RollsBackAndReturnsFailure_WhenExceptionOccurs()
        {
            var request = new RegisterRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
                Role = "employee",
            };
            using var cts = new CancellationTokenSource();

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
                .ThrowsAsync(new InvalidOperationException("Database unavailable"));

            var result = await CreateService().RegisterAsync(request, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Failure, result.Status);
            Assert.Equal("Failed to create the user", result.Error);
            _unitOfWorkMock.Verify(x => x.RollbackAsync(cts.Token), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_ReturnsSuccessWithEmptyGuid_WhenEmployeeServiceReturnsFailure()
        {
            var request = new RegisterRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
                Role = "employee",
            };
            using var cts = new CancellationTokenSource();

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
                .Callback<User, string>((user, _) => user.Id = "user-1")
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock
                .Setup(x => x.AddToRoleAsync(It.IsAny<User>(), request.Role))
                .ReturnsAsync(IdentityResult.Success);
            _employeeServiceMock
                .Setup(x => x.AddAsync(It.IsAny<CreateEmployeeDto>(), cts.Token))
                .ReturnsAsync(Result<Guid>.Failure("Employee creation failed"));
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(cts.Token)).ReturnsAsync(1);
            _unitOfWorkMock.Setup(x => x.CommitAsync(cts.Token)).Returns(Task.CompletedTask);

            var result = await CreateService().RegisterAsync(request, cts.Token);

            Assert.Equal(Application.Enums.ResultStatus.Success, result.Status);
            Assert.NotNull(result.Data);
            Assert.Equal(Guid.Empty, result.Data.EmployeeId);
            _unitOfWorkMock.Verify(x => x.CommitAsync(cts.Token), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ReturnsUnauthorized_WhenUserDoesNotExist()
        {
            var request = new LoginRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User?)null);

            var result = await CreateService().LoginAsync(request);

            Assert.Equal(Application.Enums.ResultStatus.Unauthorized, result.Status);
            Assert.Equal("Invalid credentials", result.Error);
            _tokenServiceMock.Verify(
                x => x.GenerateToken(It.IsAny<User>(), It.IsAny<IList<string>>()),
                Times.Never
            );
        }

        [Fact]
        public async Task LoginAsync_ReturnsUnauthorized_WhenPasswordIsInvalid()
        {
            var request = new LoginRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Wrong@123",
            };
            var user = new User
            {
                Id = "user-1",
                Email = request.Email,
                UserName = request.Email,
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            _userManagerMock
                .Setup(x => x.CheckPasswordAsync(user, request.Password))
                .ReturnsAsync(false);

            var result = await CreateService().LoginAsync(request);

            Assert.Equal(Application.Enums.ResultStatus.Unauthorized, result.Status);
            Assert.Equal("Invalid credentials", result.Error);
            _userManagerMock.Verify(x => x.GetRolesAsync(It.IsAny<User>()), Times.Never);
            _tokenServiceMock.Verify(
                x => x.GenerateToken(It.IsAny<User>(), It.IsAny<IList<string>>()),
                Times.Never
            );
        }

        [Fact]
        public async Task LoginAsync_Throws_WhenTokenServiceFails()
        {
            var request = new LoginRequest
            {
                Email = "kishan.singh@unthinkable.co",
                Password = "Pass@123",
            };
            var user = new User
            {
                Id = "user-1",
                Email = request.Email,
                UserName = request.Email,
            };
            var roles = new List<string> { "employee" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            _userManagerMock
                .Setup(x => x.CheckPasswordAsync(user, request.Password))
                .ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
            _tokenServiceMock
                .Setup(x => x.GenerateToken(user, roles))
                .Throws(new InvalidOperationException("Token generation failed"));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                CreateService().LoginAsync(request)
            );
        }

        private AuthService CreateService()
        {
            return new AuthService(
                _userManagerMock.Object,
                _tokenServiceMock.Object,
                _loggerMock.Object,
                _employeeServiceMock.Object,
                _unitOfWorkMock.Object
            );
        }

        private static Mock<UserManager<User>> CreateUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
                store.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!
            );
        }
    }
}
