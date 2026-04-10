using AuthTask.Domain.Entities;

namespace AuthTask.Application.Interfaces
{
    /// <summary>
    /// Generates JWT tokens for authenticated users.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates an access token for a user and their roles.
        /// </summary>
        /// <param name="user">Authenticated user.</param>
        /// <param name="roles">Assigned role names.</param>
        /// <returns>Signed JWT access token.</returns>
        string GenerateToken(User user, IList<string> roles);
    }
}
