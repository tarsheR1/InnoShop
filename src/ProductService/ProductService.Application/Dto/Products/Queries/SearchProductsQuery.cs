public record SearchProductsQuery(
        string? Name = null,
        Guid? CategoryId = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        bool? IsAvailable = null,
        string? CreatedByUserId = null
    );
