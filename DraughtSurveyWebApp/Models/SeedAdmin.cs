using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.Models
{
    public class SeedAdmin
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
