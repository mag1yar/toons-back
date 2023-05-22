using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using toons.Models.Email;
using MailKit.Net.Smtp;

namespace toons.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SendEmail(EmailDto request)
        {
            try
            {
                MimeMessage email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration["Email:Username"]));
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;
                email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

                using var smtp = new SmtpClient();
                smtp.Connect(_configuration["Email:Host"], int.Parse(_configuration["Email:Port"]), SecureSocketOptions.StartTls);
                smtp.Authenticate(_configuration["Email:Username"], _configuration["Email:Password"]);
                smtp.Send(email);
                smtp.Disconnect(true);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
