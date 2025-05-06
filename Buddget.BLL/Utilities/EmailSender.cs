using DotNetEnv;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Buddget.BLL.Utilities
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpHost = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
            _smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS");
            //_smtpUser = _configuration["smtpUser"];
            //_smtpPass = _configuration["smtpPass"];
        }


        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var fromAddress = new MailAddress(_smtpUser, "Budget App");
            var toAddress = new MailAddress(email);

            var smtp = new SmtpClient
            {
                Host = _smtpHost,
                Port = _smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
            };

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            return smtp.SendMailAsync(message);
        }
    }
}
