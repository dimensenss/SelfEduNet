using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SelfEduNet.Configurations;

namespace SelfEduNet.Services
{
    public class EmailSenderService(IOptions<EmailSMTPSettings> emailSettingsOptions)
    {

        private readonly EmailSMTPSettings _emailSettings = emailSettingsOptions.Value;

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient
            {
                Host = _emailSettings.SmtpServer,
                Port = _emailSettings.Port,
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _emailSettings.SenderEmail,
                    _emailSettings.SenderPassword
                )
            };

            await client.SendMailAsync(
                new MailMessage(_emailSettings.SenderEmail, email, subject, message)
                { IsBodyHtml = true });
        }
    }
}
