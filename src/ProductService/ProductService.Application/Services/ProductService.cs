using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Domain.ValueObjects;

namespace ProductService.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Products.GetByIdAsync(id);
        }

        public async Task<List<Product>> GetUserProductsAsync(string userId)
        {
            return await _unitOfWork.Products.GetByUserIdAsync(userId);
        }

        public async Task<ProductSearchResult> SearchProductsAsync(ProductSearchCriteria criteria)
        {
            return await _unitOfWork.Products.SearchAsync(criteria);
        }

        public async Task<Product> CreateProductAsync(CreateProductDto createDto, string userId)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(createDto.CategoryId);
            if (category == null)
                throw new ArgumentException("Category not found");

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                Description = createDto.Description,
                Price = createDto.Price,
                IsAvailable = createDto.IsAvailable,
                CategoryId = createDto.CategoryId,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            product.Category = category;
            return product;
        }

        public async Task<Product?> UpdateProductAsync(Guid id, UpdateProductDto updateDto, string userId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return null;

            if (product.CreatedByUserId != userId)
                throw new UnauthorizedAccessException("You can only update your own products");

            if (updateDto.CategoryId.HasValue && updateDto.CategoryId.Value != product.CategoryId)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(updateDto.CategoryId.Value);
                if (category == null)
                    throw new ArgumentException("Category not found");

                product.CategoryId = updateDto.CategoryId.Value;
                product.Category = category;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Name))
                product.Name = updateDto.Name;

            if (!string.IsNullOrWhiteSpace(updateDto.Description))
                product.Description = updateDto.Description;

            if (updateDto.Price.HasValue)
                product.Price = updateDto.Price.Value;

            if (updateDto.IsAvailable.HasValue)
                product.IsAvailable = updateDto.IsAvailable.Value;

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return product;
        }

        public async Task<bool> DeleteProductAsync(Guid id, string userId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
                return false;

            if (product.CreatedByUserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own products");

            await _unitOfWork.Products.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
