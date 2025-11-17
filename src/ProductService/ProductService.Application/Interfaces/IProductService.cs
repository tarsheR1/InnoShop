using ProductService.Domain.Entities;
using ProductService.Domain.ValueObjects;

namespace ProductService.Application.Interfaces
{
    public interface IProductService
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<List<Product>> GetUserProductsAsync(string userId);
        Task<ProductSearchResult> SearchProductsAsync(ProductSearchCriteria criteria);
        Task<Product> CreateProductAsync(CreateProductDto createDto, string userId);
        Task<Product?> UpdateProductAsync(Guid id, UpdateProductDto updateDto, string userId);
        Task<bool> DeleteProductAsync(Guid id, string userId);
    }
}
