using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookCatalog.Models
{
    public class Book
    {
        public int id { get; set; }
        public string title { get; set; }
        public int author_id { get; set; }
        public DateTime year_of_manufacture { get; set; }
        public string isbn { get; set; }
        public int? coverimage_id { get; set; }
        public string? description { get; set; }
        public int genre_id { get; set; }
    }
}
