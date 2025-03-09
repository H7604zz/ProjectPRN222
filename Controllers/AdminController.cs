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
    //[Authorize(Roles = "Admin")]
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

        public ActionResult ManageUser(string? keyword, int currentPage)
        {
            var listUserQuery = !string.IsNullOrEmpty(keyword)
                ? _userService.SearchUser(keyword).Where(u => u.RoleName != "Admin")
                : _userService.GetAllUsers().Where(u => u.RoleName != "Admin");

            int totalProduct = listUserQuery.Count();
            int totalPage = (int)Math.Ceiling((double)totalProduct / ITEM_PER_PAGE); // Tổng số trang

            // Đảm bảo currentPage trong khoảng hợp lệ
            currentPage = Math.Max(1, Math.Min(currentPage, totalPage));

            ViewBag.keyword = keyword;
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
                if (_userService.IsEmailExisted(model.Email))
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
                        TempData["Success"] = "User created successfully";
                        return RedirectToAction(nameof(ManageUser));
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


    }
}

