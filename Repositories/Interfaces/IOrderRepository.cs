using TameShop.Models;

namespace TameShop.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order?> GetOrderByIdAndUserIdAsync(int id, string userId);
        bool CheckStatus(Order order);
        bool DeleteOrderItem(Order order, OrderItem entity);
        Task<OrderItem?> GetOrderItemByAnimalId(int orderId, int animalId);
    }
}