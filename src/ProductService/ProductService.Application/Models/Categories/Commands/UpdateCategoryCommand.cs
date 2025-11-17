public record UpdateCategoryCommand(
    Guid CategoryId,
    string? Name = null,
    string? Description = null
);
