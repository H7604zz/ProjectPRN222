using ProjectPrn222.Models;

namespace ProjectPrn222.Service.Iterface
{
    public interface ICategoryService
    {
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(Category category);
        bool HasCategory(string categoryName);
        Category? GetCategory(int id);
        IQueryable<Category> GetAllCategories();
    }
}
