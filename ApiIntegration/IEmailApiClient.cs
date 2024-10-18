﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiIntegration
{
    public interface IEmailApiClient
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
