using Microsoft.AspNetCore.Mvc;
using TameShop.Data;
using TameShop.Models;

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

        [HttpGet("test")]
        public IActionResult Test()
        {
            var testUser = new User
            {
                UserName = "testuser",
                PasswordHash = "testpassword",
                Email = "example@gmail.com"
            };
            _context.Users.Add(testUser);
            _context.SaveChanges();
            return Ok(testUser);
        }
    }
}
