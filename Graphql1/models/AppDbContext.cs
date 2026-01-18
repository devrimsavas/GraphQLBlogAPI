using Microsoft.EntityFrameworkCore;
using Graphql1.models;

namespace Graphql1.models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; } 
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .HasOne(b => b.User)
                .WithMany(u => u.Blogs)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Blog>()
                .HasOne(b=>b.Category)
                .WithMany(c=>c.Blogs)
                .HasForeignKey(b => b.CategoryId);
                
            
        }

    }
}
