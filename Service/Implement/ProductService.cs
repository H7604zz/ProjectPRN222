using Microsoft.EntityFrameworkCore;
using ProjectPrn222.Models;
using ProjectPrn222.Models.DTO;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Service.Implement
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        public ProductService(AppDbContext context)
        {
            _context = context;
        }
		public int AddProduct(Product product)
		{
			_context.Products.Add(product);
			_context.SaveChanges();

			return product.ProductId;
		}
		public void AddPriceForProduct(ProductPrice productPrice)
        {
            _context.ProductPrices.Add(productPrice);
            _context.SaveChanges();
        }
        public void DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
        public void EditProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }
        public IQueryable<ProductViewModel> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductPrices.OrderByDescending(pp => pp.UpdateDate).Take(1))
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Image = p.Image,
                    Quanity = p.Quanity,
                    CategoryId = p.CategoryId,
                    Description = p.Description,
                    CategoryName = p.Category.CategoryName,
                    IsActive = p.IsActive,
                    Price = (decimal)(p.ProductPrices.OrderByDescending(pp => pp.UpdateDate).FirstOrDefault().Price),
                    PriceUpdateDate = p.ProductPrices.OrderByDescending(pp => pp.UpdateDate).FirstOrDefault().UpdateDate
                })
                .OrderByDescending(p => p.IsActive); ;
        }

        public ProductViewModel? GetProductById(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductPrices.OrderByDescending(pp => pp.UpdateDate).Take(1))
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Image = p.Image,
                    Quanity = p.Quanity,
                    CategoryId = p.CategoryId,
                    Description = p.Description,
                    CategoryName = p.Category.CategoryName,
                    IsActive = p.IsActive,
                    Price = (decimal)(p.ProductPrices.OrderByDescending(pp => pp.UpdateDate).FirstOrDefault().Price),
                    PriceUpdateDate = p.ProductPrices.OrderByDescending(pp => pp.UpdateDate).FirstOrDefault().UpdateDate
                })
                .FirstOrDefault(p => p.ProductId == id);
                
            return product;
        }

        public Product? GetProductModelById(int id)
        {
            return _context.Products.AsNoTracking().FirstOrDefault(p => p.ProductId == id);
        }
        public IQueryable<ProductViewModel>? SearchProduct(string keyword)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductPrices.OrderByDescending(pp => pp.UpdateDate).Take(1))
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Image = p.Image,
                    Quanity = p.Quanity,
                    CategoryId = p.CategoryId,
                    Description = p.Description,
                    CategoryName = p.Category.CategoryName,
                    IsActive = p.IsActive,
                    Price = (decimal)(p.ProductPrices.OrderByDescending(pp => pp.UpdateDate).FirstOrDefault().Price),
                    PriceUpdateDate = p.ProductPrices.OrderByDescending(pp => pp.UpdateDate).FirstOrDefault().UpdateDate
                })
                .Where(p => p.ProductName.Contains(keyword));

            return product;
        }

        public IQueryable<Category> GetAllCategories()
        {
            return _context.Categories; 
        }

        public bool HasProductName(string productName, int productId)
        {
            return _context.Products.Any(p => p.ProductName.ToLower() == productName.ToLower() && p.ProductId != productId);
        }
    }
}
