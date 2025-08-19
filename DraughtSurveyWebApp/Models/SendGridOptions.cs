using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.Models
{
    public sealed class SendGridOptions
    {
        [Required]
        public string ApiKey { get; set; } = string.Empty;
    }
}
