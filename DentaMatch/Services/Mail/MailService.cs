using SendGrid.Helpers.Mail;
using SendGrid;
using DentaMatch.Services.Mail.IServices;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace DentaMatch.Services.Mail
{
    public class MailService : IMailService
    {
        private IConfiguration _configuration;
        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {

            var apiKey = _configuration["SendGridApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("dentamatching@gmail.com", "DentaMatch");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);
        }

    }
}
