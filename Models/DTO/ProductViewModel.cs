using System.ComponentModel.DataAnnotations;

namespace ProjectPrn222.Models.DTO
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
		[Required(ErrorMessage = "Tên sản phẩm không được bỏ trống.")]
		public string ProductName { get; set; }

		public string? Image { get; set; }
		public IFormFile? ImageFile { get; set; }

		[Required(ErrorMessage = "Số lượng không được bỏ trống.")]
		[Range(1, double.MaxValue, ErrorMessage = "Số lượng tối thiệu phải lớn hơn 1.")]
		public int Quanity { get; set; }
		[Required(ErrorMessage = "Vui lòng chọn danh mục.")]
		public int CategoryId { get; set; }
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
		[Required(ErrorMessage = "Giá không được bỏ trống.")]
		[Range(1000, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 1000.")]
		public decimal Price { get; set; }
        public DateTime PriceUpdateDate { get; set; } = DateTime.Now;

		//các sản phẩm tương tự
		public IEnumerable<ProductViewModel>? SimilarProducts { get; set; }
	}
}
