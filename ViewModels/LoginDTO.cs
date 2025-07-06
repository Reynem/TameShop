namespace TameShop.ViewModels
{
    public class LoginDTO
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
    }
}
