using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectPrn222.Models;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(IUserService userService,
								  ICartService cartService,
                                  UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _cartService = cartService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Cart()
        {
			//lấy id của tài khoản
			var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var userId = user.Id;

			//lấy thông tin của sản phẩm theo userId
            var listCart = _cartService.GetCartsOfCustomer(userId);

            //tính tiền ship
            ViewBag.Shipping = 30000f;

			//giảm giá (nếu có)


			return View(listCart);
        }

        public IActionResult AddToCart()
        {
            //kiểm tra đã đăng nhập chưa
            //nếu chưa thì chuyển đến trang login


            return RedirectToAction("Index", "Home");
        }
    }
}
