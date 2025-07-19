using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TameShop.Data;

namespace TameShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CartsController : ControllerBase
    {
        private readonly TameShopDbContext _context;

        public CartsController(TameShopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCart()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not authenticated.");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid("User ID not found in claims.");
            }

            var cart = _context.Carts.FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound("Cart not found.");
            }

            return Ok(cart);
        }

        [HttpPost]
        public IActionResult CreateCart()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not authenticated.");
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Forbid("User ID not found in claims.");
            }
            var cart = new Models.Cart { UserId = userId };
            _context.Carts.Add(cart);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetCart), new { id = cart.Id }, cart);
        }
    }
}
