using UserService.Domain.Entities;

namespace UserService.Application.Models.Dto.Auth
{
    public class AuthenticationResult
    {
        public bool Result { get; }
        public string? AccessToken { get; }
        public string? RefreshToken { get; }
        public DateTime? ExpiresAt { get; }
        public string? ErrorMessage { get; }
        public User? User { get; }

        private AuthenticationResult(bool success, string? accessToken, string? refreshToken, DateTime? expiresAt, string? errorMessage, User? user)
        {
            Result = success;
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
