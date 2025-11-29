namespace UserService.Application.Interfaces
{
    public interface IEmailManagementService
    {
        Task ForgotPasswordAsync(string email, CancellationToken cancellationToken);
        Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken);
        Task<bool> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken);
        Task ResendEmailConfirmationAsync(string email, CancellationToken cancellationToken);
        Task<bool> IsEmailConfirmedAsync(Guid userId, CancellationToken cancellationToken);
    }
}
