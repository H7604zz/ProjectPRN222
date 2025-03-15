using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ProjectPrn222.Models;
using ProjectPrn222.Models.DTO;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(IUserService userService,
								  ICartService cartService,
                                  IProductService productService,
                                  UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _cartService = cartService;
            _productService = productService;
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

		[HttpPost]
		public IActionResult UpdateCart([FromBody] List<Cart> updatedCart)
		{
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Login", "Auth");
			}

			var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

			foreach (var item in updatedCart)
			{
				//check số lượng tối đa của mỗi sản phẩm
				var product = _productService.GetProductById(item.ProductId);
				var quantityInStock = product.Quanity;
				if (item.QuantityInCart > quantityInStock)
				{
					item.QuantityInCart = quantityInStock; //cập nhật thành số lượng tối đa
				}
				_cartService.UpdateCartQuantity(userId, item.ProductId, item.QuantityInCart);
			}

			return Json(new { success = true });
		}

		[HttpPost]
		public IActionResult RemoveFromCart(int productId)
		{
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Login", "Auth");
			}

			var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

			var success = _cartService.RemoveCartItem(userId, productId);

			if (success)
			{
				return Json(new { success = true });
			}

			return Json(new { success = false, message = "Không thể xóa sản phẩm." });
		}
		public IActionResult AddToCart(int productId, int quantity = 1)
        {
            //kiểm tra đã đăng nhập chưa
            //nếu chưa thì chuyển đến trang login
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
			}

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

			//check số lượng trong kho
			var product = _productService.GetProductById(productId);
			if (product == null)
			{
				return NotFound();
			}

			int productQuantityInStock = product.Quanity;

			// Kiểm tra xem sản phẩm đã có trong giỏ hàng hay chưa
			var existingCartItem = _cartService.HasProductIncart(userId, productId);
			int currentCartQuantity = existingCartItem?.QuantityInCart ?? 0; // Số lượng hiện có trong giỏ

			int newTotalQuantity = currentCartQuantity + quantity; // Tổng số lượng sau khi thêm

			// Kiểm tra nếu tổng số lượng vượt quá số lượng tồn kho
			if (newTotalQuantity > productQuantityInStock)
			{
				TempData["Warning"] = $"Số lượng trong giỏ hàng không được vượt quá {productQuantityInStock}. Hiện bạn đã có {currentCartQuantity} sản phẩm này trong giỏ.";
				return RedirectToAction("ProductDetails", "Home", new { productId = productId }); // Quay lại trang chi tiết sản phẩm với thông báo lỗi
			}

			// Nếu sản phẩm đã có trong giỏ hàng, cập nhật số lượng
			if (existingCartItem != null)
			{
				_cartService.UpdateCartQuantity(userId, productId, newTotalQuantity);
			}
			else
			{
				// Nếu chưa có trong giỏ hàng, thêm mới
				var cartItem = new Cart
				{
					UserId = userId,
					ProductId = productId,
					QuantityInCart = quantity
				};
				_cartService.AddCart(cartItem);
			}

			return RedirectToAction("Index", "Home");
		}
    }
}
