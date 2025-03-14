namespace ProjectPrn222.Models.DTO
{
	public class CartViewModel
	{
		// Thông tin từ Cart
		public int CartId { get; set; }
		public int QuantityInCart { get; set; }

		// Thông tin từ Product
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public string ProductImage { get; set; }

		// Thông tin từ ProductPrice
		public float Price { get; set; }

		// Tổng tiền cho mỗi sản phẩm trong giỏ hàng
		public float TotalPrice => Price * QuantityInCart;
	}
}
