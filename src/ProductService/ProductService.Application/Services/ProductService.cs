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

        public async Task<Product?> GetByIdAsync(GetProductByIdQuery query)
        {
            return await _unitOfWork.Products.GetByIdAsync(query.ProductId);
        }

        public async Task<List<Product>> GetUserProductsAsync(GetUserProductsQuery query)
        {
            return await _unitOfWork.Products.GetByUserIdAsync(query.UserId);
        }

        public async Task<ProductSearchResult> SearchProductsAsync(SearchProductsQuery query)
        {
            var criteria = new ProductSearchCriteria(
                Name: query.Name,
                CategoryId: query.CategoryId,
                MinPrice: query.MinPrice,
                MaxPrice: query.MaxPrice,
                IsAvailable: query.IsAvailable,
                CreatedByUserId: query.CreatedByUserId
            );

            return await _unitOfWork.Products.SearchAsync(criteria);
        }

        public async Task<Product> CreateProductAsync(CreateProductCommand command)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(command.CategoryId);
            if (category == null)
                throw new ArgumentException("Category not found");

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                IsAvailable = command.IsAvailable,
                CategoryId = command.CategoryId,
                CreatedByUserId = command.UserId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            product.Category = category;
            return product;
        }

        public async Task<Product?> UpdateProductAsync(UpdateProductCommand command, string userId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(command.ProductId);
            if (product == null)
                return null;

            if (product.CreatedByUserId != userId)
                throw new UnauthorizedAccessException("You can only update your own products");

            if (command.CategoryId.HasValue && command.CategoryId.Value != product.CategoryId)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(command.CategoryId.Value);
                if (category == null)
                    throw new ArgumentException("Category not found");

                product.CategoryId = command.CategoryId.Value;
                product.Category = category;
            }

            if (!string.IsNullOrWhiteSpace(command.Name))
                product.Name = command.Name;

            if (!string.IsNullOrWhiteSpace(command.Description))
                product.Description = command.Description;

            if (command.Price.HasValue)
                product.Price = command.Price.Value;

            if (command.IsAvailable.HasValue)
                product.IsAvailable = command.IsAvailable.Value;

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return product;
        }

        public async Task<bool> DeleteProductAsync(DeleteProductCommand command)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(command.ProductId);
            if (product == null)
                return false;

            if (product.CreatedByUserId != command.UserId)
                throw new UnauthorizedAccessException("You can only delete your own products");

            await _unitOfWork.Products.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeactivateUserProductsAsync(string userId)
        {
            var products = await _unitOfWork.Products.GetByUserIdAsync(userId);
            if (products == null)
                return true;

            foreach (var product in products)
            {
                product.IsAvailable = false;
            }

            return true;
        }
    }
}
