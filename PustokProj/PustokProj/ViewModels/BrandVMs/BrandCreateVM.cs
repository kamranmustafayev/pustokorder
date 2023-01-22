using System.ComponentModel.DataAnnotations.Schema;

namespace PustokProj.ViewModels.BrandVMs
{
    public class BrandCreateVM
    {
        public int Queue { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
