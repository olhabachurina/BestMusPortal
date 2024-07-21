using BestMusPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace BestMusPortal.Data
{
    
        public class ApplicationDbContext : IdentityDbContext<IdentityUser>
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<User> Users { get; set; }
            public DbSet<Song> Songs { get; set; }
            public DbSet<Genre> Genres { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<User>()
                    .HasMany(u => u.Songs)
                    .WithOne(s => s.User)
                    .HasForeignKey(s => s.UserId);

                modelBuilder.Entity<Genre>()
                    .HasMany(g => g.Songs)
                    .WithOne(s => s.Genre)
                    .HasForeignKey(s => s.GenreId);

                modelBuilder.Entity<User>()
                    .Property(u => u.IsApproved)
                    .HasDefaultValue(false);

                modelBuilder.Entity<User>()
                    .Property(u => u.Role)
                    .HasDefaultValue("User");
            }
        }
    }
