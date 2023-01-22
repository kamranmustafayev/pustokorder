using System.ComponentModel.DataAnnotations.Schema;

namespace PustokProj.Models
{
    public class Brand : BaseEntity
    {
        public string ImageUrl { get; set; }
        public int Queue { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
