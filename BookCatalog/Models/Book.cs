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
        public int ID { get; set; }
        public string Title { get; set; }
        public int AuthorID { get; set; }
        public DateTime YearOfManufacture { get; set; }
        public string ISBN { get; set; }
        public int CoverimageId { get; set; }
        public string Description { get; set; }
        public int GenreID { get; set; }


    }
}
