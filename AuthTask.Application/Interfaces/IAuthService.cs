using AuthTask.Application.DTOs;

namespace AuthTask.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
        Task<Result<RegisterResponse>> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default
        );
    }
}
