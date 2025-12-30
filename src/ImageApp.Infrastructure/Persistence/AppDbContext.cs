using Microsoft.EntityFrameworkCore;
using ImageApp.Domain.Entities;

namespace ImageApp.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Photo> Photos => Set<Photo>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Rating> Ratings => Set<Rating>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<User>()
        //         .HasIndex(x => x.Email).IsUnique();
        //
        //     modelBuilder.Entity<Photo>()
        //         .HasOne(p => p.Creator)
        //         .WithMany()
        //         .HasForeignKey(p => p.CreatorId);
        //
        //     modelBuilder.Entity<Rating>()
        //         .HasIndex(r => new { r.PhotoId, r.UserId }).IsUnique();
        //
        //     base.OnModelCreating(modelBuilder);
        // }
    }
}