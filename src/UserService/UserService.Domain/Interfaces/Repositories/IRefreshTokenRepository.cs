using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task<RefreshToken?> GetByJwtIdAsync(string jwtId, CancellationToken cancellationToken);
        Task<IEnumerable<RefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<RefreshToken>> GetActiveUserTokensAsync(Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<RefreshToken>> GetExpiredTokensAsync(CancellationToken cancellationToken);
        Task<bool> ExistsValidTokenAsync(string token, CancellationToken cancellationToken);
        Task<RefreshToken?> GetValidTokenAsync(string token, CancellationToken cancellationToken);
        Task<int> GetActiveTokensCountAsync(Guid userId, CancellationToken cancellationToken);
        Task DeleteRangeAsync(IEnumerable<RefreshToken> tokens, CancellationToken cancellationToken);
    }
}
