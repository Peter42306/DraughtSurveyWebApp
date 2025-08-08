using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.ViewModels
{
    public class ExcelTemplateCreateViewModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty; // Name of the template visible to users in select dropdown
        
        public bool IsPublic { get; set; } = false; // if true = available to all users
        
        public string? OwnerId { get; set; } // FK to ApplicationUser, null if public template

        [Required]
        public IFormFile? File { get; set; } // Uploaded file

        public IEnumerable<SelectListItem> Owners { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
