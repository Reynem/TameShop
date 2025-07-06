using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TameShop.Data;
using TameShop.Models;
using TameShop.ViewModels;

namespace TameShop.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly UserDbContext _context;
        public AuthController(UserDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (string.IsNullOrWhiteSpace(registerDTO.UserName) ||
                string.IsNullOrWhiteSpace(registerDTO.Password) ||
                string.IsNullOrWhiteSpace(registerDTO.Email))
            {
                return BadRequest("Invalid user data.");
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName != null && u.UserName.Equals(registerDTO.UserName, StringComparison.OrdinalIgnoreCase));

            if (existingUser != null)
            {
                return Conflict("Username already exists.");
            }

            var existingEmail = await _context.Users
               .FirstOrDefaultAsync(u => u.Email != null && u.Email.Equals(registerDTO.Email, StringComparison.OrdinalIgnoreCase));

            if (existingEmail != null)
            {
                return Conflict("Email already in use.");
            }

            var user = new User
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                PasswordHash = Utils.PasswordHashing.HashPassword(registerDTO.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return StatusCode(201, "User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (string.IsNullOrWhiteSpace(loginDTO.UserName) || string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                return BadRequest("Invalid login data.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName != null && u.UserName.Equals(loginDTO.UserName, StringComparison.OrdinalIgnoreCase));

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !Utils.PasswordHashing.VerifyPassword(loginDTO.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok("Login successful.");
        }

        [HttpPost("login-email")]
        public async Task<IActionResult> LoginByEmail([FromBody] LoginDTO loginDTO)
        {
            if (string.IsNullOrEmpty(loginDTO.Email) || string.IsNullOrEmpty(loginDTO.Password))
            {
                return BadRequest("Invalid login data");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email.Equals(loginDTO.Email, StringComparison.OrdinalIgnoreCase));

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !Utils.PasswordHashing.VerifyPassword(loginDTO.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok("Login successful");
        }

    }
}
