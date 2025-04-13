using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using w8d1_AdvancedUnitTesting.Models;
using w8d1_AdvancedUnitTesting.Services;

namespace w8d1_AdvancedUnitTesting.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        protected readonly IConfiguration Configuration;

        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            // Avoid accessing Database in constructor to prevent issues during mocking
            // If you need to log the connection string, do it in a method called explicitly
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer(Configuration?.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<Order>().HasKey(o => o.OrderId);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId);
        }
    }
}