using Microsoft.AspNetCore.Mvc;
using ProjectPrn222.Models;
using ProjectPrn222.Service.Iterface;
using System.Diagnostics;

namespace ProjectPrn222.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly int ITEM_PER_PAGE = 9;
        private int totalPage;
        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            var hotProduct = _productService.GetAllProducts().Take(3).ToList();
            return View(hotProduct);
        }

        public IActionResult ListProduct(string? keyword, int currentPage = 1)
        {
            // Bắt đầu từ truy vấn tìm kiếm nếu có keyword, ngược lại lấy tất cả sản phẩm
            var productListQuery = !string.IsNullOrEmpty(keyword)
                ? _productService.SearchProduct(keyword)
                : _productService.GetAllProducts();

            int totalProduct = productListQuery.Count(); // Tổng số sản phẩm sau khi lọc
            int totalPage = (int)Math.Ceiling((double)totalProduct / ITEM_PER_PAGE); // Tổng số trang

            // Đảm bảo currentPage trong khoảng hợp lệ
            currentPage = Math.Max(1, Math.Min(currentPage, totalPage));

            // Truyền dữ liệu cho ViewBag
            ViewBag.Keyword = keyword;
            ViewBag.CurrentPage = currentPage;
            ViewBag.CountPage = totalPage;

            // Phân trang
            var pagedProduct = productListQuery
                .Skip((currentPage - 1) * ITEM_PER_PAGE)
                .Take(ITEM_PER_PAGE)
                .ToList();

            return View(pagedProduct);
        }



        public IActionResult Details(int productId)
        {
            var productDetails = _productService.GetProductById(productId);
            return View(productDetails);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
