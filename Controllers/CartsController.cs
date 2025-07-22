using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TameShop.Models;
using TameShop.Data;
using Microsoft.EntityFrameworkCore;
using TameShop.ViewModels;

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
        public async Task<IActionResult> GetCart()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Forbid("User ID not found in claims.");
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound("Cart not found.");
            }

            var cartDto = CartDTO.AutoMapper(cart);

            return Ok(cartDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItemInCart([FromBody] CartItemDTO cartItemDTO)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                // TODO: Handle the case with SessionID
                return Unauthorized("User ID not found in claims. Please log in.");
            }

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                await _context.Carts.AddAsync(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.AnimalId == cartItemDTO.AnimalId);
            if (existingItem != null)
            {
                existingItem.Quantity += cartItemDTO.Quantity;
            } else
            {
                var newCartItem = new CartItem
                {
                    AnimalId = cartItemDTO.AnimalId,
                    Quantity = cartItemDTO.Quantity,
                    CartId = cart.CartId
                };

                cart.UpdatedAt = DateTime.UtcNow;

                cart.Items.Add(newCartItem);
            }

            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var cartDto = CartDTO.AutoMapper(cart);

            return Ok(cartDto);
        }

        [HttpDelete("{animalId}")]
        public async Task<IActionResult> RemoveItemFromCart(int animalId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in claims. Please log in.");
            }
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                return NotFound("Cart not found.");
            }
            var itemToRemove = cart.Items.FirstOrDefault(i => i.AnimalId == animalId);
            if (itemToRemove == null)
            {
                return NotFound("Item not found in cart.");
            }
            cart.Items.Remove(itemToRemove);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            var cartDto = CartDTO.AutoMapper(cart);
            return Ok(cartDto);
        }
    }
}
