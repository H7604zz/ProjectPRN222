using Microsoft.AspNetCore.Mvc;
using ProjectPrn222.Models;
using ProjectPrn222.Service.Implement;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Controllers
{
    public class StaffController : Controller
    {
        public readonly ICategoryService _categoryService;
        public StaffController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
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
                if (_categoryService.HasCategory(model.CategoryName))
                {
					TempData["Error"] = "Danh mục này đã tồn tại";
                    ViewBag.categoryName = model.CategoryName; 
					return Json(new { success = false });
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
	}
}
