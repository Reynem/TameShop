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
    public class CartsController : ControllerBase
    {
        private readonly TameShopDbContext _context;

        private const string CartSessionKey = "CartId";

        public CartsController(TameShopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await GetCartByUserIdAsync();

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var animalExists = await _context.Animals.AnyAsync(a => a.Id == cartItemDTO.AnimalId);
            if (!animalExists)
            {
                return BadRequest("Invalid AnimalId. Animal not found.");
            }

            var userId = GetValidUserAsync();

            var cart = await GetCartByUserIdAsync();

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                await _context.Carts.AddAsync(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.AnimalId == cartItemDTO.AnimalId);

            if (existingItem != null)
            {
                existingItem.Quantity += cartItemDTO.Quantity;
            }

            else
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

        [HttpPut]
        public async Task<IActionResult> UpdateItemInCart([FromBody] CartItemUpdateDTO cartItemUpdateDTO)
        {
            var cart = await GetCartByUserIdAsync();

            if (cart == null)
            {
                return NotFound("Cart not found.");
            }

            var itemToUpdate = cart.Items.FirstOrDefault(i => i.AnimalId == cartItemUpdateDTO.AnimalId);

            if (itemToUpdate == null)
            {
                return NotFound("Item not found in cart.");
            }

            itemToUpdate.Quantity = cartItemUpdateDTO.Quantity;
            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            var cartDto = CartDTO.AutoMapper(cart);

            return Ok(cartDto);
        }

        [HttpDelete("decrease")]
        public async Task<IActionResult> DecreaseItemInCart([FromBody] CartItemUpdateDTO cartItemUpdateDTO)
        {
            var cart = await GetCartByUserIdAsync();

            if (cart == null)
            {
                return NotFound("Cart not found.");
            }

            var itemToDecrease = cart.Items.FirstOrDefault(i => i.AnimalId == cartItemUpdateDTO.AnimalId);

            if (itemToDecrease == null)
            {
                return NotFound("Item not found in cart.");
            }

            if (itemToDecrease.Quantity - cartItemUpdateDTO.Quantity >= 1)
            {
                itemToDecrease.Quantity -= cartItemUpdateDTO.Quantity;
            }
            else
            {
                cart.Items.Remove(itemToDecrease);
            }
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            var cartDto = CartDTO.AutoMapper(cart);
            return Ok(cartDto);
        }

        [HttpDelete("{animalId}")]
        public async Task<IActionResult> RemoveItemFromCart(int animalId)
        {
            var cart = await GetCartByUserIdAsync();

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

        private string GetValidUserAsync() 
        { 
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                return userId;
            }

            var sessionId = HttpContext.Session.GetString(CartSessionKey);

            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString(CartSessionKey, sessionId);
            }
            
            return sessionId;
        }

        private async Task<Cart> GetCartByUserIdAsync()
        {
            try
            {
                var userId = GetValidUserAsync();

                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
                return cart;
#pragma warning restore CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
            }
            catch (UnauthorizedAccessException e)
            {
                throw new UnauthorizedAccessException("An error occurred while retrieving the cart.", e);
            }
        }
    }
}
