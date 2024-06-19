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

        public static BitmapImage GetCover(CoverImage cover)
        {
            using (var dbContext = new MyDbContext())
            {
                if (cover == null || cover.cover_data == null || cover.cover_data.Length == 0)
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
        public static CoverImage GetCoverImageFromBitmap(BitmapImage bitmapImage)
        {
            if (bitmapImage == null || bitmapImage.StreamSource == null)
                return null;

            try
            {
                byte[] imageData;
                using (var memoryStream = new MemoryStream())
                {
                    BitmapEncoder encoder = new PngBitmapEncoder(); 
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    encoder.Save(memoryStream);
                    imageData = memoryStream.ToArray();
                }

                CoverImage cover = new CoverImage
                {
                    cover_data = imageData
                };

                return cover;
            }
            catch
            {
                return null;
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
