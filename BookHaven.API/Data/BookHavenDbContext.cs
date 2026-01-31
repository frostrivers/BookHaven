using BookHaven.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BookHaven.API.Data
{
    public class BookHavenDbContext : DbContext
    {
        public BookHavenDbContext(DbContextOptions<BookHavenDbContext> options) : base(options)
        {
        }

        public DbSet<UserInfo> Users { get; set; }
        public DbSet<AuthorInfo> Authors { get; set; }
        public DbSet<GenreInfo> Genres { get; set; }
        public DbSet<BookInfo> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserInfo
            modelBuilder.Entity<UserInfo>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<UserInfo>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<UserInfo>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            // Configure AuthorInfo
            modelBuilder.Entity<AuthorInfo>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<AuthorInfo>()
                .Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Configure GenreInfo
            modelBuilder.Entity<GenreInfo>()
                .HasKey(g => g.Id);
            modelBuilder.Entity<GenreInfo>()
                .Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Configure BookInfo
            modelBuilder.Entity<BookInfo>()
                .HasKey(b => b.Id);
            modelBuilder.Entity<BookInfo>()
                .Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(255);
            modelBuilder.Entity<BookInfo>()
                .Property(b => b.ISBN)
                .HasMaxLength(13);
        }
    }
}