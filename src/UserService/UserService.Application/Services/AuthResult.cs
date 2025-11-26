using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Domain.Interfaces.Services;

namespace UserService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthenticationResult> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByUsernameAsync(username, cancellationToken);

            if (user == null || !user.IsActive)
                return AuthenticationResult.Failure("Invalid credentials");

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, password))
                return AuthenticationResult.Failure("Invalid credentials");

            // Обновляем время последнего входа
            await _unitOfWork.Users.UpdateLastLoginAsync(user.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Генерируем токены
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = await _tokenService.GenerateAndSaveRefreshTokenAsync(user.Id, cancellationToken);

            return AuthenticationResult.Success(accessToken, refreshToken, user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var userId = Guid.Parse(principal.FindFirst("uid")?.Value ?? throw new InvalidOperationException("Invalid token"));

            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null || !user.IsActive)
                return AuthenticationResult.Failure("User not found");

            var isValid = await _tokenService.ValidateRefreshTokenAsync(userId, refreshToken, cancellationToken);
            if (!isValid)
                return AuthenticationResult.Failure("Invalid refresh token");

            // Отзываем старый refresh token
            await _tokenService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);

            // Генерируем новые токены
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = await _tokenService.GenerateAndSaveRefreshTokenAsync(user.Id, cancellationToken);

            return AuthenticationResult.Success(newAccessToken, newRefreshToken, user);
        }

        public async Task RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            await _tokenService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
        }

        public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken)
        {
            await _unitOfWork.RefreshTokens.RevokeUserTokensAsync(userId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken)
        {
            return _tokenService.ValidateToken(token);
        }
    }

    public class AuthenticationResult
    {
        public bool Success { get; }
        public string? AccessToken { get; }
        public string? RefreshToken { get; }
        public DateTime? ExpiresAt { get; }
        public string? ErrorMessage { get; }
        public User? User { get; }

        private AuthenticationResult(bool success, string? accessToken, string? refreshToken, DateTime? expiresAt, string? errorMessage, User? user)
        {
            Success = success;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresAt = expiresAt;
            ErrorMessage = errorMessage;
            User = user;
        }

        public static AuthenticationResult Success(string accessToken, string refreshToken, User user)
        {
            return new AuthenticationResult(true, accessToken, refreshToken, DateTime.UtcNow.AddHours(1), null, user);
        }

        public static AuthenticationResult Failure(string errorMessage)
        {
            return new AuthenticationResult(false, null, null, null, errorMessage, null);
        }
    }
}
