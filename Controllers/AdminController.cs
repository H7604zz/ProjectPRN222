using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectPrn222.Models;
using ProjectPrn222.Models.DTO;
using ProjectPrn222.Service.Implement;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly int ITEM_PER_PAGE = 10;
        public AdminController(IUserService userService,
               UserManager<ApplicationUser> userManager,
               RoleManager<ApplicationRole> roleManager)
        {
            _userService = userService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult ManageUser(string? keyword, string? role, int currentPage)
        {
            ViewBag.Roles = _roleManager.Roles.Where(u => u.Name != "Admin").ToList();

            var listUserQuery = !string.IsNullOrEmpty(keyword)
                ? _userService.SearchUser(keyword).Where(u => u.RoleName != "Admin")
                : _userService.GetAllUsers().Where(u => u.RoleName != "Admin");

            if (!string.IsNullOrEmpty(role))
            {
                listUserQuery = listUserQuery.Where(u => u.RoleName == role);
            }

            int totalProduct = listUserQuery.Count();
            int totalPage = (int)Math.Ceiling((double)totalProduct / ITEM_PER_PAGE); // Tổng số trang

            // Đảm bảo currentPage trong khoảng hợp lệ
            currentPage = Math.Max(1, Math.Min(currentPage, totalPage));

            ViewBag.keyword = keyword;
            ViewBag.role = role;
            ViewBag.CurrentPage = currentPage;
            ViewBag.CountPage = totalPage;

            // Phân trang
            var paginatedUsers = listUserQuery
                                .Skip((currentPage - 1) * ITEM_PER_PAGE)
                                .Take(ITEM_PER_PAGE)
                                .ToList();

            return View(paginatedUsers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = _roleManager.Roles.Where(u => u.Name != "Admin").ToList();
            return PartialView("_CreateUserModal");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    ModelState.AddModelError("", "Email đã được sử dụng. Vui lòng chọn email khác.");
                }
                else
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    await _userManager.AddToRoleAsync(user, model.RoleName);
                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Tạo tài khoản thành công.";
                        //return RedirectToAction(nameof(ManageUser));
                        return Json(new { success = true});
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

            }
            ViewBag.Roles = _roleManager.Roles.Where(u => u.Name != "Admin").ToList();
            ViewBag.password = model.Password;
            ViewBag.confirmpassword = model.ConfirmPassword;
            ViewBag.email = model.Email;
            return PartialView("_CreateUserModal", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = roles.FirstOrDefault(),
                Password = user.PasswordHash,
                ConfirmPassword = user.PasswordHash
            };

            ViewBag.Roles = _roleManager.Roles.Where(u => u.Name != "Admin").ToList();
            return PartialView("_EditUserModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,UserName,Email,RoleName,Password,ConfirmPassword")] UserViewModel model)
        {
            if (id != model.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != model.UserId)
                {
                    ModelState.AddModelError("", "Email đã được sử dụng. Vui lòng chọn email khác.");
                }
                else
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    user.UserName = model.UserName;
                    user.Email = model.Email;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        var currentRoles = await _userManager.GetRolesAsync(user);
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        await _userManager.AddToRoleAsync(user, model.RoleName);
                        TempData["Success"] = "Cập nhật tài khoản thành công.";
                        //return RedirectToAction(nameof(ManageUser));
                        return Json(new { success = true });
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            ViewBag.Roles = _roleManager.Roles
                .Where(r => r.Name != "Admin")
                .Select(r => r.Name)
                .ToList();
            return PartialView("_CreateUserModal", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "ID không hợp lệ.";
                return Json(new { success = false });
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return Json(new { success = false });
            }

            try
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Xóa người dùng thành công!";
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return Json(new { success = false });
            }
            return Json(new { success = false });
        }
    }
}

