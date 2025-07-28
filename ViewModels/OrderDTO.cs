using TameShop.Models;

namespace TameShop.ViewModels
{
    public class OrderDTO
    {
        public string UserId { get; set; } = string.Empty;
        public required List<OrderItemResponseDTO> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public static OrderDTO AutoMapper(Order order)
        {
            return new OrderDTO
            {
                UserId = order.UserId,
                Items = order.Items.Select(o =>
                    new OrderItemResponseDTO
                    {
                        AnimalId = o.AnimalId,
                        Quantity = o.Quantity,
                        Price = o.Price,
                        AnimalName = o.AnimalName
                    }
                ).ToList(),
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt
            };
        }
    }
}
