using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectPrn222.Models.VNPay;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Controllers
{
	[Authorize(Roles = "Customer")]
	public class PaymentController : Controller
	{
		private readonly IVnPayService _vnPayService;
		public PaymentController(IVnPayService vnPayService)
		{

			_vnPayService = vnPayService;
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
			return View("~/Views/Customer/PaymentCallbackVnpay.cshtml", response);
		}

	}
}
