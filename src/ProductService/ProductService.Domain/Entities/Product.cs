namespace ProductService.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public Guid CategoryId { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public Category Category { get; set; } = null!;
    }
}
