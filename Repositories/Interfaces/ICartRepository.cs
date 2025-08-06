using TameShop.Models;

namespace TameShop.Data.Repositories.Interfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart?> GetByUserIdAsync(string userId);
    }
}