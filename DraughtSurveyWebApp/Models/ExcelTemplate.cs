using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.Models
{
    public class ExcelTemplate
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty; // Name of the template visible to users in select dropdown

        [Required, StringLength(500)]
        public string FilePath { get; set; } = string.Empty; 

        public bool IsPublic { get; set; } = false; // if true = availabe to all users

        // if template is not public, it belongs to a specific user
        public string? OwnerId { get; set; } // FK to ApplicationUser
        public ApplicationUser? Owner { get; set; }

        [StringLength(500)]
        public string? OriginalFileName { get; set; } // Original file name when uploaded

        [StringLength(500)]
        public string? ContentType { get; set; } // // "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"

        [Range(1, long.MaxValue)]
        public long? FileSizeBytes { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedAtUtc { get; set; }
    }
}
