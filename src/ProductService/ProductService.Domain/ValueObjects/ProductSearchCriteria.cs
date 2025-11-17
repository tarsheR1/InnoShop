namespace ProductService.Domain.ValueObjects
{
    public record ProductSearchCriteria(
        string? Name = null,
        Guid? CategoryId = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        bool? IsAvailable = null,
        string? CreatedByUserId = null
    );
}
