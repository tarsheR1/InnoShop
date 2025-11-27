using Microsoft.EntityFrameworkCore.Storage;
using UserService.Domain.Interfaces;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UsersDbContext _context;
        private IDbContextTransaction? _transaction;

        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public IRefreshTokenRepository RefreshTokens { get; }

        public UnitOfWork(
            UsersDbContext context,
            IUserRepository usersRepository,
            IRoleRepository roleRepository,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _context = context;
            Users = usersRepository;
            Roles = roleRepository;
            RefreshTokens = refreshTokenRepository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}

