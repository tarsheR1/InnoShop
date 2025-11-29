public record CreateProductCommand(
        string Name,
        string Description,
        decimal Price,
        bool IsAvailable,
        Guid CategoryId,
        string UserId
    );
