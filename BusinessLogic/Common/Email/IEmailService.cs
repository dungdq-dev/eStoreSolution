namespace BusinessLogic.Common.Email
{
    public interface IEmailService
    {
        Task SendEmail(string email, string subject, string message);
    }
}