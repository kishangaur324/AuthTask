using AuthTask.Application.DTOs;

namespace AuthTask.Application.Interfaces
{
    /// <summary>
    /// Provides authentication workflows for user registration and login.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user and returns a token payload.
        /// </summary>
        /// <param name="request">Login request data.</param>
        /// <returns>Operation result containing token details.</returns>
        Task<Result<LoginResponse>> LoginAsync(LoginRequest request);

        /// <summary>
        /// Registers a new user account and creates an employee for that user as well
        /// </summary>
        /// <param name="request">Registration request data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Operation result containing registration details.</returns>
        Task<Result<RegisterResponse>> RegisterAsync(
            RegisterRequest request,
            CancellationToken cancellationToken = default
        );
    }
}
