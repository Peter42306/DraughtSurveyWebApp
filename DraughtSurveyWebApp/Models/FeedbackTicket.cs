using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DraughtSurveyWebApp.Models
{
    public class FeedbackTicket
    {
        public int Id { get; set; }


        public string ApplicationUserId { get; set; } = null!;

        [ForeignKey(nameof(ApplicationUserId))]
        public required ApplicationUser ApplicationUser { get; set; }

        public required string UserEmail { get; set; }

        [Required, StringLength(4000, MinimumLength = 5)]
        public required string Message { get; set; }

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        [StringLength(2000)]
        public string? AdminNotes { get; set; }
    }
}
