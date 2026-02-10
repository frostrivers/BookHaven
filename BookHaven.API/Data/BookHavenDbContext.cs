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
        public DbSet<ItemTypeInfo> ItemTypes { get; set; }
        public DbSet<SellItemInfo> SellItems { get; set; }
        public DbSet<SubscriberInfo> Subscribers { get; set; }
        public DbSet<EventInfo> Events { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }

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

            // Configure ItemTypeInfo
            modelBuilder.Entity<ItemTypeInfo>()
                .HasKey(g => g.Id);
            modelBuilder.Entity<ItemTypeInfo>()
                .Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Configure SellItemInfo
            modelBuilder.Entity<SellItemInfo>()
                .HasKey(b => b.Id);
            modelBuilder.Entity<SellItemInfo>()
                .Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(255);
            modelBuilder.Entity<SellItemInfo>()
                .Property(b => b.ISBN)
                .HasMaxLength(13);
            modelBuilder.Entity<SellItemInfo>()
                .Property(b => b.CoverImage)
                .HasColumnType("LONGTEXT");

            // Configure SubscriberInfo
            modelBuilder.Entity<SubscriberInfo>()
                .HasKey(s => s.Id);
            modelBuilder.Entity<SubscriberInfo>()
                .Property(s => s.Email)
                .IsRequired()
                .HasMaxLength(255);
            modelBuilder.Entity<SubscriberInfo>()
                .HasIndex(s => s.Email)
                .IsUnique();
            modelBuilder.Entity<SubscriberInfo>()
                .Property(s => s.Name)
                .HasMaxLength(100);

            // Configure EventInfo
            modelBuilder.Entity<EventInfo>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<EventInfo>()
                .Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            modelBuilder.Entity<EventInfo>()
                .Property(e => e.EventType)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<EventInfo>()
                .Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<EventInfo>()
                .HasMany(e => e.Registrations)
                .WithOne(r => r.Event)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure EventRegistration
            modelBuilder.Entity<EventRegistration>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<EventRegistration>()
                .Property(r => r.Email)
                .IsRequired()
                .HasMaxLength(255);
            modelBuilder.Entity<EventRegistration>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<EventRegistration>()
                .HasIndex(r => new { r.EventId, r.Email })
                .IsUnique();
        }
    }
}