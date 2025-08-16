using TameShop.Models;

namespace TameShop.Repositories.Interfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart?> GetByUserIdAsync(string userId);
        Task<bool> AnimalExistsAsync(int animalId);
        void Delete(Cart entity);
    }
}