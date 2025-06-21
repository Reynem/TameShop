using Microsoft.EntityFrameworkCore;

namespace TameShop
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Animal> Animals { get; set; } = null!;

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=animals.db");
        }

    }
}
