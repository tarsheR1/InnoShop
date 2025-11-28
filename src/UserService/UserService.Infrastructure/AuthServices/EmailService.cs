using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Sockets;
using UserService.Application.Interfaces;
using UserService.Application.Models.Settings;


namespace UserService.Infrastructure.AuthServices
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly IConfiguration _configuration;

        public EmailService(
            IOptions<SmtpSettings> smtpSettings,
            IConfiguration configuration)
        {
            _smtpSettings = smtpSettings.Value;
            _configuration = configuration;
        }

        public async Task SendEmailConfirmationAsync(string email, string confirmationToken)
        {
            var confirmationLink = $"{_configuration["Frontend:BaseUrl"]}/confirm-email?token={WebUtility.UrlEncode(confirmationToken)}&email={WebUtility.UrlEncode(email)}";

            var subject = "Подтверждение email адреса";
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .button {{ display: inline-block; padding: 12px 24px; background-color: #007bff; color: white; text-decoration: none; border-radius: 4px; }}
                        .footer {{ margin-top: 20px; padding-top: 20px; border-top: 1px solid #eee; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>Подтверждение email адреса</h2>
                        <p>Для завершения регистрации подтвердите ваш email адрес, нажав на кнопку ниже:</p>
                        <p><a href='{confirmationLink}' class='button'>Подтвердить Email</a></p>
                        <p>Или скопируйте ссылку в браузер:</p>
                        <p><code>{confirmationLink}</code></p>
                        <p>Если вы не регистрировались на нашем сайте, проигнорируйте это письмо.</p>
                        <div class='footer'>
                            <p>С уважением,<br>Команда {_smtpSettings.SenderName}</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetAsync(string email, string resetToken)
        {
            var resetLink = $"{_configuration["Frontend:BaseUrl"]}/reset-password?token={WebUtility.UrlEncode(resetToken)}";

            var subject = "Восстановление пароля";
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .button {{ display: inline-block; padding: 12px 24px; background-color: #dc3545; color: white; text-decoration: none; border-radius: 4px; }}
                        .footer {{ margin-top: 20px; padding-top: 20px; border-top: 1px solid #eee; font-size: 12px; color: #666; }}
                        .warning {{ background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 12px; border-radius: 4px; margin: 15px 0; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>Восстановление пароля</h2>
                        <p>Мы получили запрос на восстановление пароля для вашего аккаунта.</p>
                        <p>Для установки нового пароля нажмите на кнопку ниже:</p>
                        <p><a href='{resetLink}' class='button'>Восстановить пароль</a></p>
                        <p>Или скопируйте ссылку в браузер:</p>
                        <p><code>{resetLink}</code></p>
                        <div class='warning'>
                            <strong>Важно:</strong> Ссылка действительна в течение 24 часов. Если вы не запрашивали восстановление пароля, проигнорируйте это письмо.
                        </div>
                        <div class='footer'>
                            <p>С уважением,<br>Команда {_smtpSettings.SenderName}</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                using var smtpClient = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port)
                {
                    EnableSsl = _smtpSettings.EnableSsl,
                    UseDefaultCredentials = _smtpSettings.UseDefaultCredentials,
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 300000
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.SenderEmail, _smtpSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                    Priority = MailPriority.Normal
                };

                mailMessage.To.Add(to);

                var plainTextBody = StripHtmlTags(body);
                var plainTextView = AlternateView.CreateAlternateViewFromString(plainTextBody, null, MediaTypeNames.Text.Plain);
                mailMessage.AlternateViews.Add(plainTextView);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                throw new Exception($"Ошибка отправки email: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка отправки email: {ex.Message}");
            }
        }

        private string StripHtmlTags(string html)
        {
            return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty)
                .Replace("&nbsp;", " ")
                .Replace("&amp;", "&")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&quot;", "\"");
        }
    }
}