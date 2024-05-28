using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ChatApi.Core.Interfaces;

namespace ChatApi.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ICryptographicManager _cryptographicManager;

        public EmailService(IConfiguration configuration, ICryptographicManager cryptographicManager)
        {
            _configuration = configuration;
            _cryptographicManager = cryptographicManager;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var client = new SmtpClient(_configuration["Email:Smtp:Host"], int.Parse(_configuration["Email:Smtp:Port"]))
            {
                Credentials = new NetworkCredential(_configuration["Email:Smtp:Username"], _configuration["Email:Smtp:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:Smtp:From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
        }
    }
}
