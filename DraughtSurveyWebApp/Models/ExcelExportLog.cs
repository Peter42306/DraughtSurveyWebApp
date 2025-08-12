namespace DraughtSurveyWebApp.Models
{
    public class ExcelExportLog
    {
        public Guid Id { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int? TemplateId { get; set; }
        public ExcelTemplate? ExcelTemplate { get; set; }

        public DateTime CreatedUtc { get; set; }
    }
}
