using ProjectPrn222.Models.VNPay;

namespace ProjectPrn222.Service.Iterface
{
	public interface IVnPayService
	{
		string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
		PaymentResponseModel PaymentExecute(IQueryCollection collections);

	}
}
