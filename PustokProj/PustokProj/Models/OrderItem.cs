namespace PustokProj.Models
{
	public class OrderItem : BaseEntity
	{
		public int OrderId { get; set; }
		public int? BookId { get; set; }
		public string BookName { get; set; }
		public double SellPrice { get; set; }
		public double CostPrice { get; set; }
		public double Discount { get; set; }
		public int Count { get; set; }
		public Order Order { get; set; }
		public Book Book { get; set; }
	}
}
