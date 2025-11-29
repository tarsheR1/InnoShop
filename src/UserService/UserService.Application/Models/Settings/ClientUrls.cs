namespace UserService.Application.Models.Settings
{
    public class ClientUrls
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string EmailConfirmationPath { get; set; } = string.Empty;
        public string PasswordResetPath { get; set; } = string.Empty;
    }
}
