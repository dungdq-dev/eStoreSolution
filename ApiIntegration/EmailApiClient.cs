using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
