namespace ProjectPrn222.Models.DTO
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public float CurrentPrice { get; set; } 
        public DateTime OrderDate { get; set; }
        public float DiscountAmount { get; set; }
        public float TotalAmount { get; set; }
        public float FinalTotal { get; set; }
        public IEnumerable<Product>? ListProducts { get; set; }
    }
}
