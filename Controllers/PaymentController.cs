using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectPrn222.Models;
using ProjectPrn222.Models.VNPay;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Controllers
{
	[Authorize(Roles = "Customer")]
	public class PaymentController : Controller
	{
		private readonly IVnPayService _vnPayService;
		private readonly IOrderService _orderService;
		private readonly ICartService _cartService;
		public PaymentController(IVnPayService vnPayService, IOrderService orderService, ICartService cartService)
		{
			_vnPayService = vnPayService;
			_orderService = orderService;
			_cartService = cartService;
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

			var cartItems = _cartService.GetCartsOfCustomer(userId);
			var orderDetails = cartItems.Select(item => new OrderDetail
			{
				OrderId = orderId,
				ProductId = item.ProductId,
				Quantity = item.QuantityInCart
			}).ToList();

			_orderService.AddOrderDetails(orderDetails);
			//xóa thông tin trong giỏ hàng
			_cartService.ClearCart(userId);

			return View("~/Views/Customer/PaymentCallbackVnpay.cshtml", response);
		}

	}
}
