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
        public DbSet<CoverImage> CoverImage { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=BookCatalogDataBase;Username=postgres;Password=123;");
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
            modelBuilder.Entity<Book>().ToTable("books");
            modelBuilder.Entity<Author>().ToTable("authors");
            modelBuilder.Entity<Genre>().ToTable("genres");
            modelBuilder.Entity<CoverImage>().ToTable("coverimage");
        }

    }
}
