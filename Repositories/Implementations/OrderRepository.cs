using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using TameShop.Data;
using TameShop.Models;
using TameShop.Repositories.Interfaces;

namespace TameShop.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly TameShopDbContext _context;

        public OrderRepository(TameShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            try
            {
                var orders = await _context.Orders
                    .Include(c => c.Items)
                    .Where(o => o.UserId == userId)
                    .ToListAsync();

                return orders;
            }
            catch (UnauthorizedAccessException e)
            {
                throw new UnauthorizedAccessException("An error occurred while retrieving orders.", e);
            }
        }
        public async Task<Order?> GetOrderByIdAndUserIdAsync(int id, string userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        }

        public Task AddAsync(Order entity)
        {
            throw new NotImplementedException();
        }

        public bool DeleteOrderItem(Order order, OrderItem entity)
        {
            return order.Items.Remove(entity);
        }
        public async Task<OrderItem?> GetOrderItemByAnimalId(int orderId, int animalId)
        {
            return await _context.OrderItems
                .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.AnimalId == animalId);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Update(Order entity)
        {
            throw new NotImplementedException();
        }

        public bool CheckStatus(Order order)
        {
            return order.Status == OrderStatus.Pending;
        }
    }
}
