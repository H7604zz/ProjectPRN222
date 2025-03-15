using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectPrn222.Helpers;
using ProjectPrn222.Models;
using ProjectPrn222.Models.DTO;
using ProjectPrn222.Service.Implement;
using ProjectPrn222.Service.Iterface;
using System.ComponentModel;
using System.Globalization;

namespace ProjectPrn222.Controllers
{
    public class StaffController : Controller
    {
        public readonly ICategoryService _categoryService;
        public readonly IProductService _productService;
        public readonly ICloudinaryService _cloudinaryService;

        private readonly int ITEM_PER_PAGE = 10;
        private int totalPage;
        public StaffController(ICategoryService categoryService,
                                IProductService productService,
                                ICloudinaryService cloudinaryService)
        {
            _categoryService = categoryService;
            _productService = productService;
            _cloudinaryService = cloudinaryService;
        }
        public IActionResult ListCategories(string? keyword)
        {
            var cateListQuery = !string.IsNullOrEmpty(keyword)
                ? _categoryService.SearchCategories(keyword)
                : _categoryService.GetAllCategories();
            ViewBag.keyword = keyword;
            return View(cateListQuery.ToList());
        }
        [HttpGet]
        public IActionResult CreateCate()
        {
            return PartialView("_CreateCateModal");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCate(Category model)
        {
            if (ModelState.IsValid)
            {
                if (_categoryService.HasCategory(model.CategoryName, model.CategoryId))
                {
                    ModelState.AddModelError("", "Danh mục này đã tồn tại.");
                    ViewBag.categoryName = model.CategoryName;
                }
                else
                {
                    TempData["Success"] = "Tạo danh mục thành công";
                    _categoryService.AddCategory(model);
                    return Json(new { success = true });
                }
            }

            return PartialView("_CreateCateModal", model);
        }

        [HttpGet]
        public IActionResult EditCate(int id)
        {
            var category = _categoryService.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }
            return PartialView("_EditCateModal", category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCate(Category model)
        {
            if (ModelState.IsValid)
            {
                if (_categoryService.HasCategory(model.CategoryName, model.CategoryId))
                {
                    ModelState.AddModelError("", "Danh mục này đã tồn tại.");
                }
                else if (_categoryService.HasCateInProducts(model.CategoryId))
                {
                    ModelState.AddModelError("", "Đang có sản phẩm trong danh mục này, không thể thay đổi trạng thái.");
                }
                else
                {
                    TempData["Success"] = "Chỉnh sửa danh mục thành công";
                    _categoryService.UpdateCategory(model);
                    return Json(new { success = true });
                }
            }

            return PartialView("_EditCateModal", model);
        }

        [HttpPost]
        public IActionResult DeleteCate(int id)
        {
            var category = _categoryService.GetCategory(id);
            if (category == null)
            {
                return Json(new { success = false, message = "Danh mục này không tồn tại." });
            }
            else if (_categoryService.HasCateInProducts(id))
            {
                return Json(new { success = false, message = "Đang có sản phẩm trong danh mục này, không thể xóa." });
            }

            else
            {
                _categoryService.DeleteCategory(category);
                TempData["Success"] = "Xóa danh mục thành công!";
                return Json(new { success = true });
            }
        }

        public IActionResult ManageProduct(string? keyword, int? categoryId, int currentPage = 1)
        {
            //list category
            ViewBag.Category = new SelectList(_productService.GetAllCategories().ToList(), "CategoryId", "CategoryName", categoryId);
            // Bắt đầu từ truy vấn tìm kiếm nếu có keyword, ngược lại lấy tất cả sản phẩm
            var productListQuery = !string.IsNullOrEmpty(keyword)
                ? _productService.SearchProduct(keyword)
                : _productService.GetAllProducts();

            //lọc theo category
            if (categoryId.HasValue)
            {
                productListQuery = productListQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            int totalProduct = productListQuery.Count(); // Tổng số sản phẩm sau khi lọc
            int totalPage = (int)Math.Ceiling((double)totalProduct / ITEM_PER_PAGE); // Tổng số trang

            // Đảm bảo currentPage trong khoảng hợp lệ
            currentPage = Math.Max(1, Math.Min(currentPage, totalPage));

            // Truyền dữ liệu cho ViewBag
            ViewBag.keyword = keyword;
            ViewBag.CurrentPage = currentPage;
            ViewBag.CountPage = totalPage;

            // Phân trang
            var pagedProduct = productListQuery
                .Skip((currentPage - 1) * ITEM_PER_PAGE)
                .Take(ITEM_PER_PAGE)
                .ToList();

            return View(pagedProduct);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            var categories = _productService.GetAllCategories().Where(c => c.IsActive == true).ToList();
            ViewBag.Category = new SelectList(categories, "CategoryId", "CategoryName");
            return View("CreateProduct");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_productService.HasProductName(model.ProductName, model.ProductId))
                {
                    ModelState.AddModelError("", "Tên sản phẩm này đã tồn tại.");
                }
                else
                {
                    string imageUrl = await _cloudinaryService.UploadImageAsync(model.ImageFile);
                    // Lưu URL vào model
                    var product = new Product
                    {
                        ProductName = model.ProductName,
                        Image = imageUrl,
                        Quanity = model.Quanity,
                        CategoryId = model.CategoryId,
                        Description = model.Description,
                        IsActive = model.IsActive,
                    };
                    int newProductId = _productService.AddProduct(product);
                    var productPrice = new ProductPrice
                    {
                        ProductId = newProductId,
                        Price = (float)model.Price,
                        UpdateDate = DateTime.Now
                    };

                    _productService.AddPriceForProduct(productPrice);

                    TempData["Success"] = "Thêm sản phẩm thành công";
                    return RedirectToAction("ManageProduct", "Staff");
                }
            }
            var categories = _productService.GetAllCategories().Where(c => c.IsActive == true).ToList();
            ViewBag.Category = new SelectList(categories, "CategoryId", "CategoryName", model.CategoryId);
            ViewBag.productname = model.ProductName;
            ViewBag.quantity = model.Quanity;
            ViewBag.price = model.Price;
            ViewBag.description = model.Description;
            return View("CreateProduct");
        }


        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var categories = _productService.GetAllCategories().Where(c => c.IsActive == true).ToList();
            ViewBag.Category = new SelectList(categories, "CategoryId", "CategoryName");
            var product = _productService.GetProductById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = _productService.GetAllCategories().Where(c => c.IsActive == true).ToList();
                ViewBag.Category = new SelectList(categories, "CategoryId", "CategoryName", model.CategoryId);
                return View(model);
            }

            var product = _productService.GetProductById(model.ProductId);
            if (product == null) return NotFound();

            //kiểm tra trùng lặp tên
            if (_productService.HasProductName(model.ProductName, model.ProductId))
            {
                ModelState.AddModelError("", "Tên sản phẩm này đã tồn tại.");
                var categories = _productService.GetAllCategories().Where(c => c.IsActive == true).ToList();
                ViewBag.Category = new SelectList(categories, "CategoryId", "CategoryName", model.CategoryId);
                return View(model);
            }

            //kiểm tra có thay đổi ảnh không
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string imageUrl = await _cloudinaryService.UploadImageAsync(model.ImageFile);
                model.Image = imageUrl;
            }
            else
            {
                model.Image = product.Image; // Giữ ảnh cũ
            }

            var productModel = new Product
            {
                ProductId = model.ProductId,
                ProductName = model.ProductName,
                Image = model.Image,
                Quanity = model.Quanity,
                CategoryId = model.CategoryId,
                Description = model.Description,
                IsActive = model.IsActive,
            };
            _productService.EditProduct(productModel);

            //có sự thay đổi về giá
            if (product.Price != model.Price)
            {
                var productPriceMode = new ProductPrice
                {
                    ProductId = model.ProductId,
                    Price = (float)model.Price,
                    UpdateDate = DateTime.Now,
                };
                _productService.AddPriceForProduct(productPriceMode);
            }
            TempData["Success"] = "Chỉnh sửa sản phẩm thành công";

            return RedirectToAction("ManageProduct");
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            //kiểm tra nếu sản phẩm này đã từng được mua thì không thể xóa
            //else if ()
            //{
               
            //}
            else
            {
                var productModel = new Product
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Image = product.Image,
                    Quanity = product.Quanity,
                    CategoryId = product.CategoryId,
                    Description = product.Description,
                    IsActive = product.IsActive,
                };
                _productService.DeleteProduct(productModel);
                TempData["Success"] = "Xóa sản phẩm thành công!";
                return Json(new { success = true }); ;
            }
        }
    }
}
