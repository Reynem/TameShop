using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TameShop.Models;

namespace TameShop.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public DbSet<Profile> Profiles { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserProfile)
                .WithOne(p => p.User)
                .HasForeignKey<Profile>(p => p.Id);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Profile>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Profile>()
                .Property(p => p.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<Profile>()
                .HasOne(p => p.User)
                .WithOne(u => u.UserProfile)
                .HasForeignKey<Profile>(p => p.Id);

            base.OnModelCreating(modelBuilder);
        }

    }
}
