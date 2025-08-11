using DraughtSurveyWebApp.Models;

namespace DraughtSurveyWebApp.ViewModels
{
    public class CargoInputViewModel
    {
        public int Id { get; set; }

        public string? CargoName { get; set; }
        public double? DeclaredWeight { get; set; }

        public string? LoadingTerminal { get; set; }
        public string? BerthNumber { get; set; }

        public string? Shipper { get; set; }
        public string? Consignee { get; set; }
        public string? DischargingPort { get; set; }

        public int InspectionId { get; set; }        
        public OperationType? OperationType { get; set; }
    }
}
