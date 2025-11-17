public record UpdateProductCommand(
       Guid ProductId,
       string? Name = null,
       string? Description = null,
       decimal? Price = null,
       bool? IsAvailable = null,
       Guid? CategoryId = null
   );