using Common.Constants;
using System.Net;
using System.Net.Mail;

namespace BusinessLogic.Common.Email
{
    public class EmailService : IEmailService
    {
        public EmailService()
        {
        }

        public Task SendEmail(string email, string subject, string message)
        {
            var mailServer = SystemConstants.EmailSettings.MailServer;
            var mailPort = SystemConstants.EmailSettings.MailPort;
            var username = SystemConstants.EmailSettings.Username;
            var password = SystemConstants.EmailSettings.Password;

            var client = new SmtpClient(mailServer, mailPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };

            var mailMessage = new MailMessage(from: username, to: email, subject, message);
            mailMessage.IsBodyHtml = true;

            return client.SendMailAsync(mailMessage);
        }
    }
}