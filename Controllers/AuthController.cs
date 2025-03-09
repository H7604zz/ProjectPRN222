using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectPrn222.Models;
using ProjectPrn222.Service.Implement;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUserService _userService;

        public AuthController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<ApplicationRole> roleManager,
                                 IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userService = userService;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    //chuyển đến trang Admin
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("ManageUser", "Admin");
                    }
                    //chuyển đến trang Staff
                    //if (roles.Contains("Staff"))
                    //{
                    //    return RedirectToAction("", "Staff");
                    //}
                }
                return RedirectToAction("Index", "Home"); 
            }
            TempData["Error"] = "Tài khoản và mật khẩu không chính xác";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            var user = new ApplicationUser { UserName = username, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                if (_userManager.FindByEmailAsync(email) != null)
                {
                    TempData["Error"] = "Email đã được sử dụng. Vui lòng chọn email khác.";
                    return View();
                }
                else
                {
                    var defaultRole = "Customer";

                    // Tạo role "Customer" nếu chưa tồn tại
                    if (!await _roleManager.RoleExistsAsync(defaultRole))
                    {
                        await _roleManager.CreateAsync(new ApplicationRole { Name = defaultRole });
                    }

                    // Gán mặc định role "Customer" cho user mới
                    await _userManager.AddToRoleAsync(user, defaultRole);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    //isPersistent: false có tác dụng Khi người dùng đóng trình duyệt, cookie sẽ bị xóa và lần sau mở lại sẽ phải đăng nhập lại.
                    TempData["Success"] = "Đăng ký tài khoản thành công";
                    return RedirectToAction("Index", "Home");
                }
            }

            foreach (var error in result.Errors)
            {
                TempData["Error"] = error.Description;
                Console.WriteLine(error.Description);
            }

            ViewBag.password = password;
            ViewBag.confirmpassword = password;
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            Console.WriteLine("Đã đăng xuất");
            return RedirectToAction("Index", "Home");
        }
    }
}
