namespace DentaMatch.Services.Mail
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string content);
    }
}
