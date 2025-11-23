using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using ProductService.Domain.ValueObjects;

namespace ProductService.Infrastructure.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductServiceDbContext _context;

        public ProductRepository(ProductServiceDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<List<Product>> GetByUserIdAsync(string userId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CreatedByUserId == userId)
                .ToListAsync();
        }

        public async Task<ProductSearchResult> SearchAsync(ProductSearchCriteria criteria)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(criteria.Name))
            {
                query = query.Where(p => p.Name.Contains(criteria.Name));
            }

            if (criteria.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == criteria.CategoryId.Value);
            }

            if (criteria.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= criteria.MinPrice.Value);
            }

            if (criteria.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= criteria.MaxPrice.Value);
            }

            if (criteria.IsAvailable.HasValue)
            {
                query = query.Where(p => p.IsAvailable == criteria.IsAvailable.Value);
            }

            if (!string.IsNullOrWhiteSpace(criteria.CreatedByUserId))
            {
                query = query.Where(p => p.CreatedByUserId == criteria.CreatedByUserId);
            }

            var totalCount = await query.CountAsync();
            var products = await query.ToListAsync();

            return new ProductSearchResult(products, totalCount);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }
    }
}
