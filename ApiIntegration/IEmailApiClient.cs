namespace ApiIntegration
{
    public interface IEmailApiClient
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}