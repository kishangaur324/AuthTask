using AuthTask.Application.DTOs;
using AuthTask.Application.Interfaces;
using AuthTask.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AuthTask.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmployeeService _employeeService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            UserManager<User> userManager,
            ITokenService tokenService,
            ILogger<AuthService> logger,
            IEmployeeService employeeService,
            IUnitOfWork unitOfWork
        )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
            _employeeService = employeeService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RegisterResponse>> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default
        )
        {
            var user = new User { UserName = request.Email, Email = request.Email };

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    var items = string.Join(", ", result.Errors.Select(x => x.Description));
                    _logger.LogError(
                        "User registration failed when creating the user. error(s): [{errors}]",
                        items
                    );

                    if (result.Errors.Any(x => x.Code == "DuplicateUserName"))
                        return Result<RegisterResponse>.Conflict(items);

                    return Result<RegisterResponse>.Failure(items);
                }

                result = await _userManager.AddToRoleAsync(user, request.Role);
                if (!result.Succeeded)
                {
                    _logger.LogError(
                        "User registration failed when adding the role. error(s): [{errors}]",
                        string.Join(",", result.Errors.Select(x => x.Description))
                    );
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return Result<RegisterResponse>.Failure(
                        string.Join(", ", result.Errors.Select(x => x.Description))
                    );
                }

                // create an employee as well
                var emplpoyeeId = await _employeeService.AddAsync(
                    new CreateEmployeeDto
                    {
                        Email = user.Email,
                        DateOfJoining = DateTime.UtcNow,
                        UserId = user.Id,
                    },
                    cancellationToken
                );
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                return Result<RegisterResponse>.Success(
                    new RegisterResponse { EmployeeId = emplpoyeeId.Data, Email = user.Email }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create the user");
                await _unitOfWork.RollbackAsync(cancellationToken);
                return Result<RegisterResponse>.Failure("Failed to create the user");
            }
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return Result<LoginResponse>.Unauthorized("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);

            var token = _tokenService.GenerateToken(user, roles);
            return Result<LoginResponse>.Success(new LoginResponse { AccessToken = token });
        }
    }
}
