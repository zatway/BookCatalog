using BookCatalog.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BookCatalog.Service
{
    public static class DataService
    {
        public static List<T> GetFullTable<T>() where T : class
        {
            using (var dbContext = new MyDbContext())
            {
                List<T> list = dbContext.Set<T>().ToList();
                return list;
            }
        }

        public static string GetAuthorName(int authorId)
        {
            using (var dbContext = new MyDbContext())
            {
                Author author = dbContext.Authors.FirstOrDefault(a => a.id == authorId);
                if (author != null)
                    return author.full_name;
                return null;
            }
        }
        public static string GetGenreName(int genreId)
        {
            using (var dbContext = new MyDbContext())
            {
                Genre genre = dbContext.Genres.FirstOrDefault(a => a.id == genreId);
                if (genre != null)
                    return genre.name;
                return null;
            }
        }

        public static BitmapImage GetCover(int coverId)
        {
            using (var dbContext = new MyDbContext())
            {
                CoverImage cover = dbContext.CoverImages.FirstOrDefault(c => c.id == coverId);
                if (cover.cover_data == null || cover.cover_data.Length == 0)
                    return null;
                try
                {
                    using (var stream = new MemoryStream(cover.cover_data))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();
                        image.Freeze();
                        return image;
                    }
                }
                catch
                {
                    return null;
                }
                
            }
        }

        public static byte[] SetImageInDB(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException("Image file not found.", imagePath);
            }

            byte[] imageData = File.ReadAllBytes(imagePath);
            return imageData;
        }

    }
}
