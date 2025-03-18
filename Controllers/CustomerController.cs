using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
	[Authorize(Roles = "Customer")]
	public class CustomerController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly IVourcherService _vourcherService;
		private readonly IVnPayService _vnPayService;
		private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(IUserService userService,
								  ICartService cartService,
                                  IProductService productService,
								  IVourcherService vocherService,
								  IVnPayService vnPayService,
                                  UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _cartService = cartService;
            _productService = productService;
			_vourcherService = vocherService;
			_vnPayService = vnPayService;
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

			//tạm tính
			var subtotal = 0f;
			foreach (var cart in listCart)
			{
				subtotal += cart.QuantityInCart * cart.Price;
			}
			//ViewBag.subtotal = subtotal;
			HttpContext.Session.SetInt32("SubTotal", (int)(subtotal));

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

			float subtotal = 0f;
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
				subtotal += item.QuantityInCart * (float)product.Price;
			}

			// Kiểm tra xem có VoucherCode không
			var voucherCode = HttpContext.Session.GetString("VoucherCode");
			if (!string.IsNullOrEmpty(voucherCode))
			{
				var voucher = _vourcherService.GetVourcher(voucherCode);
				if (voucher != null && subtotal >= voucher.MinOrderValue)
				{
					float discountAmount = subtotal * voucher.Discount / 100;
					if (voucher.MaxDiscountAmount.HasValue && discountAmount > voucher.MaxDiscountAmount.Value)
					{
						discountAmount = voucher.MaxDiscountAmount.Value;
					}
					HttpContext.Session.SetInt32("DiscountAmount", (int)discountAmount);
				}
				else
				{
					// Nếu không thỏa điều kiện giảm giá, xóa session giảm giá
					HttpContext.Session.Remove("VoucherCode");
					HttpContext.Session.Remove("DiscountAmount");
				}
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
			TempData["Success"] = $"Thêm sản phẩm {product.ProductName} thành công";
			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public IActionResult ApplyVoucher(string voucherCode)
		{
			if (string.IsNullOrWhiteSpace(voucherCode))
			{
				return Json(new { success = false, message = "Vui lòng nhập mã giảm giá!" });
			}

			// Lấy thông tin mã giảm giá từ DB
			var voucher = _vourcherService.GetVourcher(voucherCode);

			if (voucher == null)
			{
				return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn!" });
			}

			// Kiểm tra thời hạn của mã giảm giá
			if (voucher.ExpiryDate < DateOnly.FromDateTime(DateTime.Now))
			{
				return Json(new { success = false, message = "Mã giảm giá đã hết hạn!" });
			}

			// Lấy giỏ hàng của người dùng
			var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			var cartItems = _cartService.GetCartsOfCustomer(userId);
			var subtotal = 0f;
			foreach (var cartItem in cartItems)
			{
				subtotal += cartItem.QuantityInCart * cartItem.Price;
			}

			// Kiểm tra điều kiện giá trị đơn hàng tối thiểu
			if (subtotal < voucher.MinOrderValue)
			{
				return Json(new { success = false, message = $"Đơn hàng phải có giá trị tối thiểu {voucher.MinOrderValue:N0} VNĐ!" });
			}

			// Tính số tiền giảm giá
			float discountAmount = subtotal * voucher.Discount / 100;

			// Giới hạn mức giảm giá tối đa (nếu có)
			if (voucher.MaxDiscountAmount.HasValue && discountAmount > voucher.MaxDiscountAmount.Value)
			{
				discountAmount = voucher.MaxDiscountAmount.Value;
			}

			HttpContext.Session.SetString("VoucherCode", voucherCode);
			HttpContext.Session.SetInt32("DiscountAmount", (int)discountAmount);

			return Json(new { success = true, discountAmount = discountAmount, message = "Mã giảm giá đã được áp dụng!" });
		}

		public IActionResult Checkout()
		{
			ViewBag.ship = 30000; // tiền ship
			var subtotal = HttpContext.Session.GetInt32("SubTotal") ?? 0; //số tiền tạm tính

			if (subtotal <= 0)
			{
				TempData["Warning"] = "Bạn không có đơn hàng nào để thanh toán";
				return RedirectToAction("Cart");
			}

			var discountAmount = HttpContext.Session.GetInt32("DiscountAmount") ?? 0; //số tiền giảm từ vourcher
			HttpContext.Session.SetInt32("TotalAmount", (int)(subtotal - discountAmount));
			return View();
		}
	}
}
