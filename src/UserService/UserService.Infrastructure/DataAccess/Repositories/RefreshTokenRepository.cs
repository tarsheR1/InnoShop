using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Infrastructure.DataAccess.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(UsersDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _dbSet
                .Include(rt => rt.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }

        public async Task<RefreshToken?> GetByJwtIdAsync(string jwtId, CancellationToken cancellationToken)
        {
            return await _dbSet
                .Include(rt => rt.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(rt => rt.JwtId == jwtId, cancellationToken);
        }

        public async Task<IEnumerable<RefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbSet
                .Where(rt => rt.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<RefreshToken>> GetActiveUserTokensAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbSet
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<RefreshToken>> GetExpiredTokensAsync(CancellationToken cancellationToken)
        {
            return await _dbSet
                .Where(rt => rt.ExpiresAt <= DateTime.UtcNow || rt.IsRevoked)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsValidTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _dbSet
                .AnyAsync(rt =>
                    rt.Token == token &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow,
                    cancellationToken);
        }

        public async Task<RefreshToken?> GetValidTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _dbSet
                .Include(rt => rt.User)
                .ThenInclude(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(rt =>
                 rt.Token == token &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow,
                    cancellationToken);
        }

        public async Task<int> GetActiveTokensCountAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(rt =>
                    rt.UserId == userId &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow,
                    cancellationToken);
        }

        public async Task DeleteRangeAsync(IEnumerable<RefreshToken> tokens, CancellationToken cancellationToken)
        {
            _dbSet.RemoveRange(tokens);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
