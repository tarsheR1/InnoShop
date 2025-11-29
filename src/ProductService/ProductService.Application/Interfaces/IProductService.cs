using ProductService.Domain.Entities;
using ProductService.Domain.ValueObjects;

namespace ProductService.Application.Interfaces
{
    public interface IProductService
    {
        Task<Product?> GetByIdAsync(GetProductByIdQuery query);
        Task<List<Product>> GetUserProductsAsync(GetUserProductsQuery query);
        Task<ProductSearchResult> SearchProductsAsync(SearchProductsQuery query);
        Task<Product> CreateProductAsync(CreateProductCommand command);
        Task<Product?> UpdateProductAsync(UpdateProductCommand command, string userId);
        Task<bool> DeleteProductAsync(DeleteProductCommand command);
    }
}
