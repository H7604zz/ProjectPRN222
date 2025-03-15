using ProjectPrn222.Models;
using ProjectPrn222.Models.DTO;

namespace ProjectPrn222.Service.Iterface
{
	public interface ICartService
	{
		void AddCart(Cart cart);
		void UpdateCart(Cart cart);
		void DeleteCart(Cart cart);
		Cart? GetCartById(int id);
		IQueryable<CartViewModel> GetCartsOfCustomer(string userId);
		Cart? HasProductIncart(string userId, int productId);
	}
}
