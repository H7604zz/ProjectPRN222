using Microsoft.AspNetCore.Mvc;
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
    }
}
