using BookCatalog.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace BookCatalog.Service
{
    public class MyDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<CoverImage> CoverImages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=BookCatalogDB;Username=postgres;Password=123;");
        }

        public bool TestConnection()
        {
            try
            {
                return this.Database.CanConnect();
            }
            catch (Exception ex)
            {
                // Логирование ошибки (если необходимо)
                Console.WriteLine("Ошибка подключения к базе данных: " + ex.Message);
                return false;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("books");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ISBN).HasMaxLength(13);
                entity.Property(e => e.YearOfManufacture).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasOne(e => e.Author)
                      .WithMany(a => a.Books)
                      .HasForeignKey(e => e.AuthorId);
                entity.HasOne(e => e.Genre)
                      .WithMany(g => g.Books)
                      .HasForeignKey(e => e.GenreId);
                entity.HasOne(e => e.CoverImage)
                      .WithMany(c => c.Books)
                      .HasForeignKey(e => e.CoverImageId);
            });

            modelBuilder.Entity<Author>(entity =>
            {
                entity.ToTable("authors");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("genres");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            });

            modelBuilder.Entity<CoverImage>(entity =>
            {
                entity.ToTable("coverimages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImageUrl).IsRequired();
            });
        }
    }
}
