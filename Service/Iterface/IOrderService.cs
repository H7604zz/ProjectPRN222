using ProjectPrn222.Models;
using ProjectPrn222.Models.DTO;

namespace ProjectPrn222.Service.Iterface
{
	public interface IOrderService
	{
		int AddOrder(Order order);
		void AddOrderDetails(IEnumerable<OrderDetail> orderDetails);
		public List<MonthlyRevenueModel> GetRevenueInMonth();
		IQueryable<OrderViewModel>? HistoryOrder(string userId);
    }
}
