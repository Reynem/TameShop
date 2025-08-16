namespace TameShop.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        void Update(T entity);
        Task<int> SaveChangesAsync();
    }
}