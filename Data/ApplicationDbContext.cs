using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Data
{
    // Updated to use ApplicationUser instead of IdentityUser
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        // Add DbSet for ApplicationUser for better query access
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Add precision configuration for UnitPrice
            modelBuilder.Entity<Product>()
                .Property(p => p.UnitPrice)
                .HasPrecision(18, 2);

            // Configure ApplicationUser
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.FullName)
                .HasMaxLength(100);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Department)
                .HasMaxLength(100);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.JobTitle)
                .HasMaxLength(100);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.EmployeeId)
                .HasMaxLength(50);
        }
    }
}