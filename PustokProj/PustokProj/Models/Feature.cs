using System.ComponentModel.DataAnnotations;

namespace PustokProj.Models
{
    public class Feature : BaseEntity
    {
        [MaxLength(25)]
        public string Title { get; set; }
        [MaxLength(40)]
        public string Description { get; set; }
        public string Icon { get; set; }
    }
}
