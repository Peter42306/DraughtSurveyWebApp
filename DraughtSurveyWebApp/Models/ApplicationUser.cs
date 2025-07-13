using Microsoft.AspNetCore.Identity;

namespace DraughtSurveyWebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; } = true;        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public int LoginCount { get; set; } = 0;
        public string? AdminNote { get; set; }
        
        public List<Inspection> Inspections { get; set; } = new();
    }
}
