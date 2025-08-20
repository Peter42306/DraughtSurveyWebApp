using DraughtSurveyWebApp.Interfaces;
using DraughtSurveyWebApp.Models;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DraughtSurveyWebApp.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridClient _sendGridClient;
        private readonly EmailOptions _emailOptions;
        private readonly ILogger<SendGridEmailSender> _logger;

        public SendGridEmailSender(
            IOptions<SendGridOptions> sendGridOption,
            IOptions<EmailOptions> emailOptions,
            ILogger<SendGridEmailSender> logger)
        {
            if (string.IsNullOrWhiteSpace(sendGridOption.Value.ApiKey))
            {
                throw new InvalidOperationException("SendGrid ApiKey is missing. Put it in user-secret/env.");
            }

            if (string.IsNullOrWhiteSpace(emailOptions.Value.From))
            {
                throw new InvalidOperationException("Email.From is missning in configuration");
            }

            _sendGridClient = new SendGridClient(sendGridOption.Value.ApiKey);
            _emailOptions = emailOptions.Value;
            _logger = logger;
        }


        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var msg = MailHelper.CreateSingleEmail(
                new EmailAddress(_emailOptions.From, _emailOptions.FromName),
                new EmailAddress(to),
                subject,
                plainTextContent: null,
                htmlContent: body);

            var resp = await _sendGridClient.SendEmailAsync(msg);
            if ((int)resp.StatusCode >= 300)
            {
                var details = await resp.Body.ReadAsStringAsync();
                _logger.LogError("SendGrid failed for {To} / {Subject}: {Status} {Body}", 
                    to, subject, resp.StatusCode, details);
                throw new InvalidOperationException($"SendGrid failed: {resp.StatusCode}");
            }            
        }
    }
}
