namespace UserService.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(string to, string confirmationToken);
        Task SendPasswordResetAsync(string to, string resetToken);
    }
}
