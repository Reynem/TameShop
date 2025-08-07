using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TameShop.Services.Interfaces;
using TameShop.ViewModels;

namespace TameShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly ICartsService _cartsService;

        public CartsController(ICartsService cartsService)
        {
            _cartsService = cartsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = _cartsService.GetValidUserId(User, HttpContext);
            var cart = await _cartsService.GetCartByUserIdAsync(userId);

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

            try
            {
                var userId = _cartsService.GetValidUserId(User, HttpContext);
                var cartDto = await _cartsService.CreateOrUpdateCartItemAsync(userId, cartItemDTO);

                if (cartDto == null)
                {
                    return NotFound("Cart not found.");
                }

                return Ok(cartDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateItemInCart([FromBody] CartItemUpdateDTO cartItemUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _cartsService.GetValidUserId(User, HttpContext);
            var cartDto = await _cartsService.UpdateCartItemQuantityAsync(userId, cartItemUpdateDTO);

            if (cartDto == null)
            {
                return NotFound("Cart or item not found.");
            }

            return Ok(cartDto);
        }

        [HttpDelete("decrease")]
        public async Task<IActionResult> DecreaseItemInCart([FromBody] CartItemUpdateDTO cartItemUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _cartsService.GetValidUserId(User, HttpContext);
            var cartDto = await _cartsService.DecreaseCartItemQuantityAsync(userId, cartItemUpdateDTO);

            if (cartDto == null)
            {
                return NotFound("Cart or item not found.");
            }

            return Ok(cartDto);
        }

        [HttpDelete("{animalId}")]
        public async Task<IActionResult> RemoveItemFromCart(int animalId)
        {
            var userId = _cartsService.GetValidUserId(User, HttpContext);
            var cartDto = await _cartsService.RemoveCartItemAsync(userId, animalId);

            if (cartDto == null)
            {
                return NotFound("Cart or item not found.");
            }

            return Ok(cartDto);
        }
    }
}