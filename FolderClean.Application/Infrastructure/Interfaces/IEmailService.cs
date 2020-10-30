namespace FolderClean.Application.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        bool SendEmailToTarget(string subject, string body);
        bool SendEmail(string toEmail, string subject, string body);
    }
}