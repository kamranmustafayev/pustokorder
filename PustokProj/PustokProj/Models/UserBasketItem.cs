namespace PustokProj.Models
{
	public class UserBasketItem : BaseEntity
	{
		public string UserId { get; set; }
		public int BookId { get; set; }
		public int Count { get; set; }
		public AppUser User { get; set; }
		public Book Book { get; set; }
	}
}
