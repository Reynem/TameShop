using System.Security.Claims;
using TameShop.Models;
using TameShop.ViewModels;

namespace TameShop.Services.Interfaces
{
    public interface ICartsService
    {
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<CartDTO?> CreateOrUpdateCartItemAsync(string userId, CartItemDTO cartItemDTO);
        Task<CartDTO?> UpdateCartItemQuantityAsync(string userId, CartItemUpdateDTO cartItemUpdateDTO);
        Task<CartDTO?> DecreaseCartItemQuantityAsync(string userId, CartItemUpdateDTO cartItemUpdateDTO);
        Task<CartDTO?> RemoveCartItemAsync(string userId, int animalId);
        string GetValidUserId(ClaimsPrincipal user, HttpContext httpContext);
    }
}