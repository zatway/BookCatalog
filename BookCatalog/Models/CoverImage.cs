using System;
using System.Drawing;
using System.IO;

namespace BookCatalog.Models
{
    public class CoverImage
    {
        public int id { get; set; }
        public byte[] cover_data { get; set; }
    }
}
