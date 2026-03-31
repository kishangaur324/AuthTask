using Microsoft.AspNetCore.Identity;

namespace AuthTask.Domain.Entities
{
    public class User : IdentityUser
    {
        public bool IsActive { get; set; }
    }
}
