using DMSMicroservice.AuthService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DMSMicroservice.AuthService.Data
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.FirstName)
                .HasMaxLength(250);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.LastName)
                .HasMaxLength(250);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.DateOfBirth);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.Address)
                .HasMaxLength(500);

            modelBuilder.Entity<ApplicationUser>()
               .Property(e => e.City)
               .HasMaxLength(100);

            modelBuilder.Entity<ApplicationUser>()
              .Property(e => e.State)
              .HasMaxLength(150);

            modelBuilder.Entity<ApplicationUser>()
             .Property(e => e.Country)
             .HasMaxLength(100);

            modelBuilder.Entity<ApplicationUser>()
            .Property(e => e.CreatedAt);

            modelBuilder.Entity<ApplicationUser>()
            .Property(e => e.UpdatedAt);

        }
    }
}
