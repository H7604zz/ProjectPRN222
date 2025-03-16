using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectPrn222.Helpers;
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
        private readonly IConfiguration _configuration;
        private readonly EmailSender _emailSender;

        public AuthController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<ApplicationRole> roleManager,
                                 IUserService userService,
                                 IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userService = userService;
            _configuration = configuration;
            _emailSender = new EmailSender(_configuration);
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                TempData["Error"] = "Tài khoản không tồn tại.";
                return View();
            }
            // Kiểm tra xem email đã được xác nhận chưa
            if (!user.EmailConfirmed)
            {
                TempData["Error"] = "Email của bạn chưa được xác nhận. Vui lòng kiểm tra email để xác nhận tài khoản.";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
            if (result.Succeeded)
            {
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    //chuyển đến trang Admin
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("ManageUser", "Admin");
                    }
                    //chuyển đến trang Staff
                    if (roles.Contains("Staff"))
                    {
                        return RedirectToAction("ManageProduct", "Staff");
                    }
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
            if (await _userManager.FindByEmailAsync(email) != null)
            {
                TempData["Error"] = "Email đã được sử dụng. Vui lòng chọn email khác.";
                ViewBag.email = email;
                ViewBag.password = password;
                ViewBag.confirmpassword = password;
                return View();
            }
            var user = new ApplicationUser { UserName = username, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var defaultRole = "Customer";

                // Tạo role "Customer" nếu chưa tồn tại
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = defaultRole });
                }

                // Gán mặc định role "Customer" cho user mới
                await _userManager.AddToRoleAsync(user, defaultRole);

                // Tạo token xác nhận email
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token }, Request.Scheme);

                // Gửi email xác nhận
                var emailSubject = "Xác nhận tài khoản của bạn";
                var emailBody = $"Vui lòng nhấp vào liên kết sau để xác nhận tài khoản của bạn: <a href='{confirmationLink}'>Xác nhận email</a>";
                await _emailSender.SendEmailAsync(user.Email, emailSubject, emailBody);

                TempData["Success"] = "Tài khoản đã được tạo. Vui lòng kiểm tra email để xác nhận.";
                return RedirectToAction("Login", "Auth");
            }
            
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            Console.WriteLine("Đã đăng xuất");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("Liên kết không hợp lệ.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["Success"] = "Xác nhận email thành công! Bạn có thể đăng nhập ngay bây giờ.";
                return RedirectToAction("Login", "Auth");
            }

            TempData["Error"] = "Xác nhận email thất bại.";
            return RedirectToAction("Login", "Auth");
        }

    }
}
