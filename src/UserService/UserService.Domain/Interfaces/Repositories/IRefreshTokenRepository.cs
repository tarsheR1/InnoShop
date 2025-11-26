using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken?> GetByJwtIdAsync(string jwtId);
        Task<IEnumerable<RefreshToken>> GetUserTokensAsync(int userId);
        Task RevokeUserTokensAsync(int userId);
        Task RevokeTokenAsync(string token);
        Task<bool> IsTokenValidAsync(string token);
        Task CleanExpiredTokensAsync();
    }
}
