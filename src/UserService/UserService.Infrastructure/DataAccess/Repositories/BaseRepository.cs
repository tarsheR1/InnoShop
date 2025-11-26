using Microsoft.EntityFrameworkCore;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Infrastructure.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly UsersDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(UsersDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public virtual void Update(T entity, CancellationToken cancellationToken)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(T entity, CancellationToken cancellationToken)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbSet.FindAsync(id , cancellationToken) != null;
        }
    }
}
