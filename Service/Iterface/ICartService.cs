using ProjectPrn222.Models;
using ProjectPrn222.Models.DTO;

namespace ProjectPrn222.Service.Iterface
{
	public interface ICartService
	{
		void AddCart(Cart cart);
		IQueryable<CartViewModel> GetCartsOfCustomer(string userId);
		Cart? HasProductIncart(string userId, int productId);
		void UpdateCartQuantity(string userId, int productId, int quantity);
		bool RemoveCartItem(string? userId, int productId);
		void ClearCart(string userId);
	}
}
