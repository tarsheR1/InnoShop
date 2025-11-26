using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, CancellationToken cancellationToken);
        string GenerateRefreshToken(CancellationToken cancellationToken);
        Task<string> GenerateAndSaveRefreshTokenAsync(int userId, string jwtId, CancellationToken cancellationToken);
        Task<bool> ValidateRefreshTokenAsync(string refreshToken, string jwtId, CancellationToken cancellationToken);
        Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task CleanExpiredRefreshTokensAsync(CancellationToken cancellationToken);
    }
}
