using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                return Json(new { success = false, message = "Đang có sản phẩm trong danh mục này, không thể xóa." });
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
    }
}
