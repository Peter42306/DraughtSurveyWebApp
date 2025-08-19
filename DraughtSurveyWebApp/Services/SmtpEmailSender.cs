using DraughtSurveyWebApp.Interfaces;
using DraughtSurveyWebApp.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace DraughtSurveyWebApp.Services
{
    public sealed class SmtpEmailSender : IEmailSender
    {
        private readonly EmailOptions _emailOptions;
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(
            IOptions<EmailOptions> emailOptions,
            IOptions<SmtpSettings> smtpOptions, 
            ILogger<SmtpEmailSender> logger)
        {
            _emailOptions = emailOptions.Value;
            _smtpSettings = smtpOptions.Value;
            _logger=logger;            
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                Timeout = 10000
            };

            using var mail = new MailMessage()
            {
                From = new MailAddress(_emailOptions.From, _emailOptions.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8
            };
            mail.To.Add(to);

            try
            {
                await client.SendMailAsync(mail);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP send failed to {to}", to);
                throw;
            }

                   
            
        }
    }
}
