using System.ComponentModel.DataAnnotations;

namespace PustokProj.Models
{
    public class Genre : BaseEntity
    {
        [MaxLength(25)]
        public string Name { get; set; }
    }
}
