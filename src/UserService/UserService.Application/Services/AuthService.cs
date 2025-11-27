using UserService.Application.Dto.Auth;
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

        public async Task<AuthenticationResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);

            if (user == null || !user.IsActive)
                return AuthenticationResult.Failure("Invalid credentials");

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, password))
                return AuthenticationResult.Failure("Invalid credentials");

            await _unitOfWork.Users.UpdateLastLoginAsync(user.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var accessToken = _tokenService.GenerateAccessToken(user, cancellationToken);
            var refreshToken = await _tokenService.GenerateAndSaveRefreshTokenAsync(user.Id, cancellationToken);

            return AuthenticationResult.Success(accessToken, refreshToken, user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken, cancellationToken);
            var userId = Guid.Parse(principal.FindFirst("uid")?.Value ?? throw new InvalidOperationException("Invalid token"));

            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null || !user.IsActive)
                return AuthenticationResult.Failure("User not found");

            var isValid = await _tokenService.ValidateRefreshTokenAsync(userId, refreshToken, cancellationToken);
            if (!isValid)
                return AuthenticationResult.Failure("Invalid refresh token");

            await _tokenService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);

            var newAccessToken = _tokenService.GenerateAccessToken(user, cancellationToken);
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

        public async Task<bool> ValidateTokenAsync(string token, Guid userId, CancellationToken cancellationToken)
        {
            return await _tokenService.ValidateRefreshTokenAsync(userId, token, cancellationToken);
        }
    }
}
