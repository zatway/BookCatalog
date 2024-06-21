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
    /// <summary>
    /// Сервис для управления основными функциями базы данных
    /// </summary>
    public static class DataService
    {
        /// <summary>
        /// Получение все таблицы
        /// </summary>
        /// <typeparam name="T">Тип модели, таблицу которой требуется получить</typeparam>
        public static ObservableCollection<T> GetFullTable<T>() where T : class
        {
            using (var dbContext = new MyDbContext())
            {
                return new ObservableCollection<T>(dbContext.Set<T>().ToList());
            }
        }

        /// <summary>
        /// Получение изображения из массива байтов
        /// </summary>
        /// <param name="coverImageInBytes">массив байтов</param>
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

        /// <summary>
        /// Сортировка, поиск книг и управление страницами через функцию в базе данных
        /// </summary>
        /// <param name="searchQuery">строка поиска</param>
        /// <param name="selectedFilterComboBoxItem">элемент комбобокса в котором хранится название фильтра</param>
        /// <param name="pageNumber">номер страницы</param>
        /// <param name="pageSize">количество строк, помещенных на одной странице</param>
        public static ObservableCollection<Book> SearchAndFilter(string searchQuery, ComboBoxItem selectedFilterComboBoxItem, int pageNumber, int pageSize)
        {
            string selectedFilter = selectedFilterComboBoxItem?.Content?.ToString();

            using (var dbContext = new MyDbContext())
            {
                var books = dbContext.Books.FromSqlInterpolated($@"
            SELECT * FROM public.filter_books(
                {searchQuery},
                {selectedFilter},
                {pageNumber},
                {pageSize}
            )
        ").ToList();
                foreach(Book book in books)
                {
                    book.Author = dbContext.Authors.FirstOrDefault(a => a.Id == book.AuthorId);
                    book.Genre = dbContext.Genres.FirstOrDefault(a => a.Id == book.GenreId);
                }
                return new ObservableCollection<Book>(books);
            }
        }

        /// <summary>
        /// Удаление книги из базы данных
        /// </summary>
        /// <param name="selectBook">Выбранная книга</param>
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
