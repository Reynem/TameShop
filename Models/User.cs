using Microsoft.AspNetCore.Identity;

namespace TameShop.Models
{
    public class User : IdentityUser
    {
        public virtual Profile? UserProfile { get; set; }
    }
}
