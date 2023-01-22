using System.ComponentModel.DataAnnotations;

namespace PustokProj.Models
{
	public class AppUser : IdentityUser
	{
		[MaxLength(256)]
		public string FullName { get; set; }
	}
}
