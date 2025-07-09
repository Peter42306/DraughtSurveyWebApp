namespace DraughtSurveyWebApp.Models
{
    public class CargoInput
    {
        public int Id { get; set; }                

        public string? CargoName { get; set; }
        public double? DeclaredWeight { get; set; }
        public string? LoadingTerminal { get; set; }
        public string? BerthNumber { get; set; }

        public int InspectionId { get; set; }
        public required Inspection Inspection { get; set; }
    }
}
