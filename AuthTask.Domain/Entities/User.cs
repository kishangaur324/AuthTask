using Microsoft.AspNetCore.Identity;

namespace AuthTask.Domain.Entities
{
    /// <summary>
    /// Application user entity extending ASP.NET Core Identity user.
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// Gets or sets whether this user account is active.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
