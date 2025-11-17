using ProductService.Data;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.DataAccess.Repositories;

namespace ProductService.Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProductServiceDbContext _context;

        public UnitOfWork(ProductServiceDbContext context)
        {
            _context = context;
            Products = new ProductRepository(context);
            Categories = new CategoryRepository(context);
        }

        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
