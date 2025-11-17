public record DeleteProductCommand(
        Guid ProductId,
        string UserId
    );