using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectPrn222.Models;
using ProjectPrn222.Models.VNPay;
using ProjectPrn222.Service.Implement;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Controllers
{
	[Authorize(Roles = "Customer")]
	public class PaymentController : Controller
	{
		private readonly IVnPayService _vnPayService;
		private readonly IOrderService _orderService;
		private readonly ICartService _cartService;
		private readonly IProductService _productService;
		public PaymentController(IVnPayService vnPayService, 
								IOrderService orderService, 
								ICartService cartService,
								IProductService productService)
		{
			_vnPayService = vnPayService;
			_orderService = orderService;
			_cartService = cartService;
			_productService = productService;
		}
		[HttpPost]
		public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
		{
			var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

			return Redirect(url);
		}
		[HttpGet]
		public IActionResult PaymentCallbackVnpay()
		{
			var response = _vnPayService.PaymentExecute(Request.Query);

			// Kiểm tra giao dịch có thành công không
			if (response.VnPayResponseCode != "00")
			{
				return View("~/Views/Customer/PaymentFailed.cshtml", response);
			}
			var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
			var orderModel = new Order
			{
				UserId = userId,
				OrderDate = DateTime.Now,
				TotalAmount = HttpContext.Session.GetInt32("SubTotal") ?? 0,
				Status = 00,
				PaymentMethod = "Vnpay",
				DiscountAmount = HttpContext.Session.GetInt32("DiscountAmount") ?? 0,
				FinalTotal = HttpContext.Session.GetInt32("TotalAmount") ?? 0,
			};
			int orderId = _orderService.AddOrder(orderModel);

			var cartItems = _cartService.GetCartsOfCustomer(userId).ToList();
			var orderDetails = cartItems.Select(item => new OrderDetail
			{
				OrderId = orderId,
				ProductId = item.ProductId,
				Quantity = item.QuantityInCart,
				Price = item.Price,
			}).ToList();

			_orderService.AddOrderDetails(orderDetails);

            // Trừ số lượng sản phẩm
            foreach (var item in cartItems)
            {
                var product = _productService.GetProductModelById(item.ProductId);
                if (product != null && product.Quanity >= item.QuantityInCart)
                {
                    product.Quanity -= item.QuantityInCart;
                    _productService.EditProduct(product);
                }
            }

            //xóa thông tin trong giỏ hàng
            _cartService.ClearCart(userId);
            HttpContext.Session.Remove("VoucherCode");
            HttpContext.Session.Remove("DiscountAmount");
            return View("~/Views/Customer/PaymentCallbackVnpay.cshtml", response);
		}

	}
}
