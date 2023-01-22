
namespace PustokProj.Models
{
	public class Order : BaseEntity
	{
		public string? UserId { get; set; }
		public string FullName { get; set; }
		public string Country { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string Address1 { get; set; }
		public string? Address2 { get; set; }
		public string City { get; set; }
		public string ZipCode { get; set; }
		public string Note { get; set; }
		public double TotalPrice { get; set; }
		public DateTime OrderedAt { get; set; }
		public AppUser User { get; set; }
		
	}
}
