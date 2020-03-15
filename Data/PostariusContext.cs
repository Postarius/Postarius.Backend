using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class PostariusContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Media> Media { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=127.0.0.1;Database=Postarius;Username=asp;Password=asp");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Owner)
                .WithMany(o => o.Posts);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Followings)
                .WithOne(s => s.Follower);

            modelBuilder.Entity<Subscription>()
                .HasIndex(s => new {s.FollowedId, s.FollowerId})
                .IsUnique();
        }
    }
}