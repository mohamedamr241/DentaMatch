namespace DentaMatch.Services.Mail.IServices
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string content);
    }
}
