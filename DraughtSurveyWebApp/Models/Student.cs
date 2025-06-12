using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Photo")]
        public string? PhotoPath { get; set; }

        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

    }
}
