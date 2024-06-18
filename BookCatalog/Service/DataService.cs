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
                Author author = dbContext.Authors.FirstOrDefault(a => a.Id == authorId);
                if (author != null)
                    return author.FullName;
                return null;
            }
        }

        public static BitmapImage GetCover(int coverId)
        {
            using (var dbContext = new MyDbContext())
            {
                CoverImage cover = dbContext.CoverImages.FirstOrDefault(c => c.Id == coverId);
                if (cover.CoverData == null || cover.CoverData.Length == 0)
                    return null;
                using (var stream = new MemoryStream(cover.CoverData))
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
