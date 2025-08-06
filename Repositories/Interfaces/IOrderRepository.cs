using TameShop.Models;

namespace TameShop.Data.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order?> GetOrderByIdAndUserIdAsync(int id, string userId);
    }
}