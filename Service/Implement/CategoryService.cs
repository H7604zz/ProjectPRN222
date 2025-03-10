using ProjectPrn222.Models;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Service.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        public CategoryService(AppDbContext context)
        {
            _context = context;
        }
        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void DeleteCategory(Category category)
        {
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }
        public void UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
        }
        public Category? GetCategory(int id)
        {
            return _context.Categories.FirstOrDefault(x => x.CategoryId == id);
        }

        public bool HasCategory(string categoryName, int currentCategoryId)
        {
            return _context.Categories.Any(x =>
                x.CategoryName.ToUpper() == categoryName.ToUpper() &&
                x.CategoryId != currentCategoryId);
        }

        public IQueryable<Category> GetAllCategories()
        {
            return _context.Categories;
        }
        public IQueryable<Category> SearchCategories(string keyword)
        {
            return _context.Categories.Where(c=>c.CategoryName.Contains(keyword));
        }
        public bool HasCateInProducts(int id)
        {
            return _context.Products.Any(c => c.CategoryId == id);
        }
    }
}
