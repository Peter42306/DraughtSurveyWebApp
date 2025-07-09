using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.Models
{
    public class SeedAdmin
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
