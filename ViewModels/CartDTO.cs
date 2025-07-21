using TameShop.Models;

namespace TameShop.ViewModels
{
    public class CartDTO
    {
        public required string CartId { get; set; }
        public required string UserId { get; set; }
        public List<CartItemDTO>? Items { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public static CartDTO AutoMapper(Cart cart)
        {
            var cartDto = new CartDTO
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                Items = cart.Items.Select(i => new CartItemDTO
                {
                    CartId = i.CartId,
                    Quantity = i.Quantity,
                    AnimalId = i.AnimalId
                }).ToList(),
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt
            };

            return cartDto;
        }
    }
}
