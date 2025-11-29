
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces;

namespace UserService.Application.Services
{
    public class EmailManagementService : IEmailManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _passwordHasher;
        public EmailManagementService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IEmailService emailService) 
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }
        public async Task ForgotPasswordAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
            if (user == null) 
            {
                return;
            }

            var resetToken = GenerateSecureToken();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(24);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _emailService.SendPasswordResetAsync(email, resetToken);
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByPasswordResetTokenAsync(token, cancellationToken);
            if (user == null || user.PasswordResetTokenExpires < DateTime.UtcNow)
                return false;

            user.PasswordHash = _passwordHasher.HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
            if (user == null || user.EmailConfirmationToken != token)
                return false;

            user.EmailConfirmed = true;
            user.EmailConfirmationToken = null;
            user.IsActive = true; 
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task ResendEmailConfirmationAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
            if (user == null || user.EmailConfirmed) return;

            var confirmationToken = GenerateSecureToken();
            user.EmailConfirmationToken = confirmationToken;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _emailService.SendEmailConfirmationAsync(email, confirmationToken);
        }

        public async Task<bool> IsEmailConfirmedAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            return user?.EmailConfirmed ?? false;
        }

        private string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}
