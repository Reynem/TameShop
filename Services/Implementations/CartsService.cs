using TameShop.Models;
using TameShop.Repositories.Interfaces;
using TameShop.Services.Interfaces;
using TameShop.ViewModels;
using System.Security.Claims;

namespace TameShop.Services.Implementations
{
    public class CartsService : ICartsService
    {
        private readonly ICartRepository _cartRepository;
        private const string CartSessionKey = "CartId";

        public CartsService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _cartRepository.GetByUserIdAsync(userId);
        }

        public async Task<CartDTO?> CreateOrUpdateCartItemAsync(string userId, CartItemDTO cartItemDTO)
        {
            var animalExists = await _cartRepository.AnimalExistsAsync(cartItemDTO.AnimalId);
            if (!animalExists)
            {
                throw new ArgumentException("Invalid AnimalId. Animal not found.");
            }

            var cart = await GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _cartRepository.AddAsync(cart);
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.AnimalId == cartItemDTO.AnimalId);

            if (existingItem != null)
            {
                existingItem.Quantity += cartItemDTO.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    AnimalId = cartItemDTO.AnimalId,
                    Quantity = cartItemDTO.Quantity
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.SaveChangesAsync();

            return CartDTO.AutoMapper(cart);
        }

        public async Task<CartDTO?> UpdateCartItemQuantityAsync(string userId, CartItemUpdateDTO cartItemUpdateDTO)
        {
            var cart = await GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return null;
            }

            var itemToUpdate = cart.Items.FirstOrDefault(i => i.AnimalId == cartItemUpdateDTO.AnimalId);

            if (itemToUpdate == null)
            {
                return null;
            }

            itemToUpdate.Quantity = cartItemUpdateDTO.Quantity;
            cart.UpdatedAt = DateTime.UtcNow;

            _cartRepository.Update(cart);
            await _cartRepository.SaveChangesAsync();

            return CartDTO.AutoMapper(cart);
        }

        public async Task<CartDTO?> DecreaseCartItemQuantityAsync(string userId, CartItemUpdateDTO cartItemUpdateDTO)
        {
            var cart = await GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return null;
            }

            var itemToDecrease = cart.Items.FirstOrDefault(i => i.AnimalId == cartItemUpdateDTO.AnimalId);

            if (itemToDecrease == null)
            {
                return null;
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
            _cartRepository.Update(cart);
            await _cartRepository.SaveChangesAsync();

            return CartDTO.AutoMapper(cart);
        }

        public async Task<CartDTO?> RemoveCartItemAsync(string userId, int animalId)
        {
            var cart = await GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return null;
            }

            var itemToRemove = cart.Items.FirstOrDefault(i => i.AnimalId == animalId);
            if (itemToRemove == null)
            {
                return null;
            }

            cart.Items.Remove(itemToRemove);
            cart.UpdatedAt = DateTime.UtcNow;

            _cartRepository.Update(cart);
            await _cartRepository.SaveChangesAsync();

            return CartDTO.AutoMapper(cart);
        }

        public string GetValidUserId(ClaimsPrincipal user, HttpContext httpContext)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                return userId;
            }

            var sessionId = httpContext.Session.GetString(CartSessionKey);

            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                httpContext.Session.SetString(CartSessionKey, sessionId);
            }

            return sessionId;
        }
    }
}