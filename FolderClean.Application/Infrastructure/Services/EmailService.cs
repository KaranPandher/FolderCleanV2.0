using System.Net;
using System.Net.Mail;
using FolderClean.Application.Infrastructure.Interfaces;
using FolderClean.Application.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace FolderClean.Application.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailOption> _options;

        public EmailService(IOptions<EmailOption> options)
        {
            _options = options;
        }

        public bool SendEmailToTarget(string subject, string body)
        {
            return SendEmail(_options.Value.TargetEmail, subject, body);
        }

        public bool SendEmail(string email, string subject, string body)
        {
            var client = new SmtpClient(_options.Value.Host, _options.Value.Port)
            {
                Credentials = new NetworkCredential(_options.Value.Email, _options.Value.Password),
                EnableSsl = true
            };
            client.UseDefaultCredentials = true;
            client.Send(_options.Value.Email, email, subject, body);
            return true;

        }
    }
}