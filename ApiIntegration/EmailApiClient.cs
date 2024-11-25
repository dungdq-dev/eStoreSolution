namespace ApiIntegration
{
    public class EmailApiClient : IEmailApiClient
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            throw new NotImplementedException();
        }
    }
}