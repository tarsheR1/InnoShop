using UserService.Application.Models.Dto.Auth;

namespace UserService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthenticationResult> AuthenticateAsync(string username, string password, CancellationToken cancellationToken);
        Task<AuthenticationResult> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken);
        Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken);
        Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task<bool> ValidateTokenAsync(string token, Guid userId, CancellationToken cancellationToken);
    }
}