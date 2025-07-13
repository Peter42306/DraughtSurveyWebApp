namespace DraughtSurveyWebApp.Models
{
    public class CargoInput
    {
        public int Id { get; set; }                

        public string? CargoName { get; set; }
        public double? DeclaredWeight { get; set; }

        public string? Shipper { get; set; }
        public string? Consignee { get; set; }
        public string? LoadingTerminal { get; set; }
        public string? BerthNumber { get; set; }
        public string? DischargingPort { get; set; }

        public int InspectionId { get; set; }
        public required Inspection Inspection { get; set; }
    }
}
