using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Category?> GetByIdAsync(Guid id);
        Task<List<Category>> GetAllCategoriesAsync(GetAllCategoriesQuery query);
        Task<Category> CreateCategoryAsync(CreateCategoryCommand command);
        Task<Category?> UpdateCategoryAsync(UpdateCategoryCommand command);
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
