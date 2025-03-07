namespace ProjectPrn222.Models.DTO
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Image { get; set; }
        public int Quanity { get; set; }
        public int CategoryId { get; set; }
        public string? Description { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public DateTime PriceUpdateDate { get; set; }

        //các sản phẩm tương tự
		public IEnumerable<ProductViewModel> SimilarProducts { get; set; }
	}
}
