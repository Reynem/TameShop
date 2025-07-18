using System.ComponentModel.DataAnnotations;

namespace TameShop.ViewModels
{
    public class LoginDTO
    {
        public string? UserName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }
    }
}
