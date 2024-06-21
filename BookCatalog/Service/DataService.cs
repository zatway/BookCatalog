using BookCatalog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BookCatalog.Service
{
    public static class DataService
    {
        public static ObservableCollection<T> GetFullTable<T>() where T : class
        {
            using (var dbContext = new MyDbContext())
            {
                return new ObservableCollection<T>(dbContext.Set<T>().ToList());
            }
        }

        public static BitmapImage GetCover(byte[] coverImageInBytes)
        {
            try
            {
                using (var stream = new MemoryStream(coverImageInBytes))
                {
                    BitmapImage CoverImageBitmap = new BitmapImage();
                    CoverImageBitmap.BeginInit();
                    CoverImageBitmap.StreamSource = new MemoryStream(coverImageInBytes);
                    CoverImageBitmap.EndInit();
                    return CoverImageBitmap;
                }
            }
            catch
            {
                return null;
            }
        }


        public static ObservableCollection<Book> StartSearch(string searchQuery, int pageNumber, int pageSize)
        {
            if (searchQuery != null)
            {
                using (var dbContext = new MyDbContext())
                {
                    return new ObservableCollection<Book>(dbContext.Books
                        .FromSqlRaw(@"
                    SELECT * FROM search_books(@searchQuery, @pageNumber, @pageSize)",
                            new NpgsqlParameter("@searchQuery", $"%{searchQuery}%"),
                            new NpgsqlParameter("@pageNumber", pageNumber),
                            new NpgsqlParameter("@pageSize", pageSize))
                        .Select(b => new Book
                        {
                            Id = b.Id,
                            Title = b.Title,
                            Author = b.Author,
                            YearOfManufacture = b.YearOfManufacture,
                            ISBN = b.ISBN,
                            Genre = b.Genre
                        }).ToList()
                        );
                }
            }
            return null;
        }

        public static ObservableCollection<Book> ApplyFilter(ComboBoxItem selectedFilter, int pageNumber, int pageSize)
        {
            if (selectedFilter != null)
            {
                using (var dbContext = new MyDbContext())
                {
                    string selectedFilterContent = selectedFilter?.Content.ToString();
                    return new ObservableCollection<Book>(dbContext.Books
                        .FromSqlRaw("SELECT * FROM filter_books(@selectedFilter, @pageNumber, @pageSize)",
                            new NpgsqlParameter("@selectedFilter", selectedFilterContent),
                            new NpgsqlParameter("@pageNumber", pageNumber),
                            new NpgsqlParameter("@pageSize", pageSize))
                        .Select(b => new Book
                        {
                            Id = b.Id,
                            Title = b.Title,
                            Author = b.Author,
                            YearOfManufacture = b.YearOfManufacture,
                            ISBN = b.ISBN,
                            Genre = b.Genre
                        }).ToList()
                        );
                }
            }
            return null;
        }

        public static ObservableCollection<Book> PagenatedOutput(int pageNumber, int pageSize)
        {
            using (var dbContext = new MyDbContext())
            {
                IQueryable<Book> query = dbContext.Books;
                int skip = (pageNumber - 1) * pageSize;
                query = query.Skip(skip).Take(pageSize);

                return new ObservableCollection<Book>(query.Select(b => new Book
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    YearOfManufacture = b.YearOfManufacture,
                    ISBN = b.ISBN,
                    Genre = b.Genre
                }).ToList()
            );
            }
        }
        public static void RemoveBookForDB(Book selectBook)
        {
            using (var dbContext = new MyDbContext())
            {
                Book book = dbContext.Books.FirstOrDefault(b => b.Id == selectBook.Id);
                dbContext.Books.Remove(book);
                dbContext.SaveChanges();
            }
        }
    }
}
