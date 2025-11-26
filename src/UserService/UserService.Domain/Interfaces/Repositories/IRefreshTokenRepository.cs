using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task<RefreshToken?> GetByJwtIdAsync(string jwtId, CancellationToken cancellationToken);
        Task<IEnumerable<RefreshToken>> GetUserTokensAsync(Guid userId, CancellationToken cancellationToken);
        Task RevokeUserTokensAsync(Guid userId, CancellationToken cancellationToken);
        Task RevokeTokenAsync(string token, CancellationToken cancellationToken);
        Task<bool> IsTokenValidAsync(string token, CancellationToken cancellationToken);
        Task CleanExpiredTokensAsync(CancellationToken cancellationToken);
    }
}
