namespace PustokProj.ViewModels.CartVMs
{
	public class OrderCM
	{
		public List<BasketItemVM>? BasketItemVMs { get; set; }
		public string FullName { get; set; }
		public string Country { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string Address1 { get; set; }
		public string? Address2 { get; set; }
		public string City { get; set; }
		public string ZipCode { get; set; }
		public string? Note { get; set; }
	}
}
