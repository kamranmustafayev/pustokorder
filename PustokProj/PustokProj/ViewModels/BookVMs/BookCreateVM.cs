using PustokProj.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PustokProj.ViewModels.BookVMs
{
    public class BookCreateVM
    {
        public int GenreId { get; set; }
        public int AuthorId { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        public string ProductCode { get; set; }
        public double CostPrice { get; set; }
        public double SellPrice { get; set; }
        public double Discount { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsNew { get; set; }
        [NotMapped]
        public IFormFile PosterImageFile { get; set; }
        [NotMapped]
        public IFormFile HoverImageFile { get; set; }
        [NotMapped]
        public List<IFormFile>? ImageFiles { get; set; }
    }
}
