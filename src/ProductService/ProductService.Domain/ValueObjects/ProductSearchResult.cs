using ProductService.Domain.Entities;

namespace ProductService.Domain.ValueObjects
{
    public record ProductSearchResult(
        List<Product> Products,
        int TotalCount
    );
}
