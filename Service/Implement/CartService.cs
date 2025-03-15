using Microsoft.EntityFrameworkCore;
using ProjectPrn222.Models;
using ProjectPrn222.Models.DTO;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Service.Implement
{
	public class CartService : ICartService
	{
		private readonly AppDbContext _context;
		public CartService(AppDbContext context)
		{
			_context = context;
		}
		public void AddCart(Cart cart)
		{
			_context.Carts.Add(cart);
			_context.SaveChanges();	
		}

		public IQueryable<CartViewModel> GetCartsOfCustomer(string userId)
		{
			return _context.Carts.Select(c => new CartViewModel{
				CartId = c.CartId,
				QuantityInCart = c.QuantityInCart,
				ProductId = c.Product.ProductId,
				ProductName = c.Product.ProductName,
				ProductImage = c.Product.Image,
				Price = (float)(c.Product.ProductPrices.OrderByDescending(pp => pp.UpdateDate).FirstOrDefault().Price),
			});
		}
		public Cart? HasProductIncart(string userId, int productId)
		{
			return _context.Carts.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
		}
		public void UpdateCartQuantity(string userId, int productId, int quantity)
		{
			var cartItem =  _context.Carts.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);
				
			if (cartItem != null)
			{
				cartItem.QuantityInCart = quantity;
			    _context.SaveChanges();
			}
		}

		public bool RemoveCartItem(string? userId, int productId)
		{
			var cartItem = _context.Carts.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

			if (cartItem != null)
			{
				_context.Carts.Remove(cartItem);
				_context.SaveChanges();
				return true;
			}

			return false;
		}
	}
}
