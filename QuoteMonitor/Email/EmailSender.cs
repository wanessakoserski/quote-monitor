using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace QuoteMonitor.Email
{
    internal class EmailSender
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly EmailsBase _emailsBase;
        private readonly string _hostEmail;
        private readonly string _hostPassword;

        public EmailSender(IOptions<SmtpSettings> smtpOptions,
                           IOptions<EmailsBase> emailsOptions)
        {
            _smtpSettings = smtpOptions.Value;
            _emailsBase = emailsOptions.Value;

            _hostEmail = Environment.GetEnvironmentVariable("HOST_EMAIL")
            ?? throw new InvalidOperationException("Problemas com a conexão com o host - email");

            _hostPassword = Environment.GetEnvironmentVariable("HOST_PASSWORD")
                ?? throw new InvalidOperationException("Problemas com a conexão com o host - password");
        }

        public async Task SendAsync(Message message)
        {
            using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_hostEmail, _hostPassword)
            };

            using var mailMessage = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress(_hostEmail, _smtpSettings.Name),
                Subject = message.Subject,
                Body = message.Body
            };

            mailMessage.To.Add(_hostEmail);

            foreach (var email in _emailsBase.Emails)
            {
                mailMessage.Bcc.Add(email);
            }

            await client.SendMailAsync(mailMessage);
        }
    }
}
