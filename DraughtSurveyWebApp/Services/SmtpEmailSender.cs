using DraughtSurveyWebApp.Interfaces;
using DraughtSurveyWebApp.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace DraughtSurveyWebApp.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<SmtpSettings> _logger;

        public SmtpEmailSender(
            IOptions<SmtpSettings> smtpOptions, 
            ILogger<SmtpSettings> logger)
        {
            _smtpSettings = smtpOptions.Value;
            _logger=logger;            
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true
            };

            var mail = new MailMessage()
            {
                From = new MailAddress(_smtpSettings.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            await client.SendMailAsync(mail);            
            
        }
    }
}
