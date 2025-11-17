using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Domain.ValueObjects;

namespace ProductService.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Categories.GetByIdAsync(id);
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.Categories.GetAllAsync();
        }

        public async Task<Category> CreateCategoryAsync(CreateCategoryDto createDto)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Description = createDto.Description
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return category;
        }

        public async Task<Category?> UpdateCategoryAsync(Guid id, UpdateCategoryDto updateDto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return null;

            if (!string.IsNullOrWhiteSpace(updateDto.Name))
                category.Name = updateDto.Name;

            if (!string.IsNullOrWhiteSpace(updateDto.Description))
                category.Description = updateDto.Description;

            await _unitOfWork.Categories.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return category;
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return false;

            var productsInCategory = await _unitOfWork.Products.SearchAsync(new ProductSearchCriteria
            {
                CategoryId = id
            });

            if (productsInCategory.TotalCount > 0)
                throw new InvalidOperationException("Cannot delete category with existing products");

            await _unitOfWork.Categories.DeleteAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}