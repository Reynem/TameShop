using System.Security.Claims;
using TameShop.Models;
using TameShop.ViewModels;

namespace TameShop.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrdersByUserAsync(string userId);
        Task<OrderDTO?> AddOrderItemAsync(int orderId, string userId, OrderItemDTO dto);
        Task<OrderDTO?> DeleteOrderItemAsync(int orderId, string userId, OrderItemDTO dto);
        Task<OrderDTO?> CompleteOrderAsync(int orderId, string userId);
        Task<OrderDTO?> CancelOrderAsync(int orderId, string userId);
        Task<List<Order>> GetAllOrdersAsync();
    }
}
