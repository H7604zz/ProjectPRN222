using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectPrn222.Models;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AuthController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
                var defaultRole = "Customer";

                // Tạo role "Customer" nếu chưa tồn tại
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = defaultRole});
                }

                // Gán mặc định role "Customer" cho user mới
                await _userManager.AddToRoleAsync(user, defaultRole);
                await _signInManager.SignInAsync(user, isPersistent: false);
                //isPersistent: false có tác dụng Khi người dùng đóng trình duyệt, cookie sẽ bị xóa và lần sau mở lại sẽ phải đăng nhập lại.

                TempData["Success"] = "Đăng ký tài khoản thành công";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                TempData["Error"] = error.Description;
                Console.WriteLine(error.Description);
            }

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
