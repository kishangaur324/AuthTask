using AuthTask.Domain.Entities;

namespace AuthTask.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user, IList<string> roles);
    }
}
