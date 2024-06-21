using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;

namespace BookCatalog.Models
{
    public class CoverImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Это указывает, что Id будет генерироваться базой данных
        public int Id { get; set; }
        public byte[] ImageData { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
