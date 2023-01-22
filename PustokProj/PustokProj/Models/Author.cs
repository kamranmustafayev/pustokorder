using System.ComponentModel.DataAnnotations;

namespace PustokProj.Models
{
    public class Author : BaseEntity
    {
        [MaxLength(50)]
        public string FullName { get; set; }
    }
}
