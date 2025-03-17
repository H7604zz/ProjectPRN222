using ProjectPrn222.Models;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Service.Implement
{
	public class OrderService : IOrderService
	{
		private readonly AppDbContext _context;
		public OrderService(AppDbContext context)
		{
			_context = context;
		}
		public int AddOrder(Order order)
		{
			_context.Orders.Add(order);
			_context.SaveChanges();
			
			return order.OrderId;
		}

		public void AddOrderDetails(IEnumerable<OrderDetail> orderDetails)
		{
			_context.OrderDetails.AddRange(orderDetails);
			_context.SaveChanges();
		}
	}
}
