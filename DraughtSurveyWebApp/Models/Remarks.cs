namespace DraughtSurveyWebApp.Models
{
    public class Remarks
    {
        public int Id { get; set; }

        public int InspectionId { get; set; }
        public required Inspection Inspection { get; set; }

        public string? Text { get; set; }        
    }
}
