using System.ComponentModel.DataAnnotations.Schema;

namespace PustokProj.ViewModels.BrandVMs
{
    public class BrandUpdateVM
    {
        public int Id { get; set; }
        public int Queue { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
