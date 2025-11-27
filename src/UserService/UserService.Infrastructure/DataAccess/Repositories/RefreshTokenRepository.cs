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
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked, cancellationToken);
        }

        public async Task<RefreshToken?> GetByJwtIdAsync(string jwtId, CancellationToken cancellationToken)
        {
            return await _dbSet
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.JwtId == jwtId && !rt.IsRevoked, cancellationToken);
        }

        public async Task<IEnumerable<RefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbSet
                .Where(rt => rt.UserId == userId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task RevokeUserTokensAsync(Guid userId, CancellationToken cancellationToken)
        {
            var tokens = await _dbSet
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
                _dbSet.Update(token);
            }
        }

        public async Task RevokeTokenAsync(string token, CancellationToken cancellationToken)
        {
            var refreshToken = await _dbSet
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked, cancellationToken);

            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                refreshToken.RevokedAt = DateTime.UtcNow;
                _dbSet.Update(refreshToken);
            }
        }

        public async Task<bool> IsTokenValidAsync(string token, CancellationToken cancellationToken)
        {
            var refreshToken = await _dbSet
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

            return refreshToken != null &&
                   !refreshToken.IsRevoked &&
                   refreshToken.ExpiresAt > DateTime.UtcNow;
        }

        public async Task CleanExpiredTokensAsync(CancellationToken cancellationToken)
        {
            var expiredTokens = await _dbSet
                .Where(rt => rt.ExpiresAt <= DateTime.UtcNow || rt.IsRevoked)
                .ToListAsync(cancellationToken);

            if (expiredTokens.Any())
            {
                _dbSet.RemoveRange(expiredTokens);
            }
        }

        public async Task<RefreshToken?> GetValidTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _dbSet
                .Include(rt => rt.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(rt =>
                    rt.Token == token &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow,
                    cancellationToken);
        }

        public async Task<int> GetActiveTokensCountAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _dbSet
                .CountAsync(rt =>
                    rt.UserId == userId &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow,
                    cancellationToken);
        }
    }
}