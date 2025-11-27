using System.Security.Claims;
using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, CancellationToken cancellationToken);
        string GenerateRefreshToken(CancellationToken cancellationToken);
        Task<string> GenerateAndSaveRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken);
        Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task CleanExpiredRefreshTokensAsync(CancellationToken cancellationToken);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token, CancellationToken cancellationToken);
    }
}
