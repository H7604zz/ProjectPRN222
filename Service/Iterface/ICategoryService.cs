using ProjectPrn222.Models;

namespace ProjectPrn222.Service.Iterface
{
    public interface ICategoryService
    {
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
        bool HasCategory(string categoryName, int currentCategoryId);
        Category? GetCategory(int id);
        IQueryable<Category> GetAllCategories();
        IQueryable<Category> SearchCategories(string keyword);
        bool HasCateInProducts(int id);
    }
}
