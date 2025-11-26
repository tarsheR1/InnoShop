using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        Task<string> GenerateAndSaveRefreshTokenAsync(int userId, string jwtId);
        Task<bool> ValidateRefreshTokenAsync(string refreshToken, string jwtId);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task CleanExpiredRefreshTokensAsync();
    }
}
