using Microsoft.EntityFrameworkCore;
using TameShop.Data;
using TameShop.Models;
using TameShop.Repositories.Interfaces;

namespace TameShop.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly TameShopDbContext _context;

        public CartRepository(TameShopDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Animal)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<IEnumerable<Cart>> GetAllAsync()
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Animal)
                .ToListAsync();
        }

        public async Task AddAsync(Cart entity)
        {
            await _context.Carts.AddAsync(entity);
        }

        public void Update(Cart entity)
        {
            _context.Carts.Update(entity);
        }

        public void Delete(Cart entity)
        {
            _context.Carts.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> AnimalExistsAsync(int animalId)
        {
            return await _context.Animals.AnyAsync(a => a.Id == animalId);
        }
    }
}