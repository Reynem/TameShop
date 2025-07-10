using Microsoft.EntityFrameworkCore;
using TameShop.Models;

namespace TameShop.Data
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Animal> Animals { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseNpgsql("Host=localhost;Port=5434;Database=tameshop;Username=kerim;Password=admin1234");
        //    }
        //}
    }
}
