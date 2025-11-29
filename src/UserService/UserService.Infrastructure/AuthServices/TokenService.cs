using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserService.Application.Models.Settings;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Domain.Interfaces.Services;

namespace UserService.Infrastructure.AuthServices
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;

        public TokenService(IUnitOfWork unitOfWork, IOptions<JwtSettings> jwtOptions)
        {
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtOptions.Value;

            if (string.IsNullOrEmpty(_jwtSettings.Secret))
                throw new InvalidOperationException("JWT SecretKey is not configured");

            if (_jwtSettings.Secret.Length < 32)
                throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long");
        }


        public string GenerateAccessToken(User user, CancellationToken cancellationToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new[]
            {
                new Claim("uid", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken(CancellationToken cancellationToken)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<string> GenerateAndSaveRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = GenerateRefreshToken(cancellationToken),
                JwtId = Guid.NewGuid().ToString(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                IsUsed = false,
                IsRevoked = false
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return refreshToken.Token;
        }

        public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken)
        {
            var token = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken, cancellationToken);
            return token != null &&
                   token.UserId == userId &&
                   !token.IsUsed &&
                   !token.IsRevoked &&
                   token.ExpiresAt > DateTime.UtcNow;
        }

        public async Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var token = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken, cancellationToken);
            token.RevokedAt = DateTime.UtcNow;
            token.IsRevoked = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task CleanExpiredRefreshTokensAsync(CancellationToken cancellationToken)
        {
            var tokens = await _unitOfWork.RefreshTokens.GetExpiredTokensAsync(cancellationToken);
            await _unitOfWork.RefreshTokens.DeleteRangeAsync(tokens, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, CancellationToken cancellationToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false, 
                ClockSkew = TimeSpan.Zero
            }, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
