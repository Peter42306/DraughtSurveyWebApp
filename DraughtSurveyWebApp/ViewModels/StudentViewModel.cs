using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.ViewModels
{
    public class StudentViewModel
    {
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string? ExistingPhotoPath { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
