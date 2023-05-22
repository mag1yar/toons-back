using toons.Models.Email;

namespace toons.Services.Email
{
    public interface IEmailService
    {
        bool SendEmail(EmailDto request);
    }
}
