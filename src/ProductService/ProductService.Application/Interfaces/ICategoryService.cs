using ProductService.Domain.Entities;

namespace ProductService.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Category?> GetByIdAsync(Guid id);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> CreateCategoryAsync(CreateCategoryDto createDto);
        Task<Category?> UpdateCategoryAsync(Guid id, UpdateCategoryDto updateDto);
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
