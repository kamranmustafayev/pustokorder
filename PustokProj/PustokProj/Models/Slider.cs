using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PustokProj.Models
{
    public class Slider : BaseEntity
    {
        [MaxLength(50)]
        public string Title1 { get; set; }
        [MaxLength(50)]
        public string Title2 { get; set; }
        public string ImageUrl { get; set; }
        public string RedirectUrl { get; set; }
        [MaxLength(30)]
        public string ButtonText { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        public int Queue { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
