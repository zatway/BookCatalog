using System;
using System.Drawing;
using System.IO;

namespace BookCatalog.Models
{
    public class CoverImage
    {
        public int Id { get; set; }
        public byte[] ImageUrl { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
