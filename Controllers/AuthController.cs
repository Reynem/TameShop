using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TameShop.Data;
using TameShop.Models;
using TameShop.ViewModels;
using TameShop.JWT;

namespace TameShop.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly UserDbContext _context;
        private readonly TokenProvider _tokenProvider;

        public AuthController(UserDbContext context, TokenProvider tokenProvider)
        {
            _context = context;
            _tokenProvider = tokenProvider;
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
                .FirstOrDefaultAsync(u =>
                    u.UserName != null &&
                    EF.Functions.ILike(u.UserName, registerDTO.UserName));

            if (existingUser != null)
            {
                return Conflict("Username already exists.");
            }

            var existingEmail = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email != null &&
                    EF.Functions.ILike(u.Email, registerDTO.Email));

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

            string token = _tokenProvider.Create(user);

            return StatusCode(201, token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (string.IsNullOrWhiteSpace(loginDTO.UserName) || string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                return BadRequest("Invalid login data.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.UserName != null &&
                    EF.Functions.ILike(u.UserName, loginDTO.UserName));

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) ||
                !Utils.PasswordHashing.VerifyPassword(loginDTO.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password.");
            }

            string token = _tokenProvider.Create(user);

            return Ok(token);
        }

        [HttpPost("login-email")]
        public async Task<IActionResult> LoginByEmail([FromBody] LoginDTO loginDTO)
        {
            if (string.IsNullOrEmpty(loginDTO.Email) || string.IsNullOrEmpty(loginDTO.Password))
            {
                return BadRequest("Invalid login data");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email != null &&
                    EF.Functions.ILike(u.Email, loginDTO.Email));

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) ||
                !Utils.PasswordHashing.VerifyPassword(loginDTO.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password.");
            }

            string token = _tokenProvider.Create(user);

            return Ok(user);
        }
    }
}
