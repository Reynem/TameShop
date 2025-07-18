using System.ComponentModel.DataAnnotations;

namespace TameShop.ViewModels
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "User name is required.")]
        [StringLength(16, ErrorMessage = "Must be between 3 and 16 characters", MinimumLength = 3)]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Must be more than 8 characters", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$", 
            ErrorMessage = "Password must contain uppercase, lowercase, number, and special character")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public required string Email { get; set; }
    }
}
