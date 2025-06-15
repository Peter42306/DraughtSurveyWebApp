using DraughtSurveyWebApp.Models;

namespace DraughtSurveyWebApp.ViewModels
{
    public class CargoInputViewModel
    {
        public int Id { get; set; }

        public string CargoName { get; set; } = string.Empty;
        public double DeclaredWeight { get; set; }
        public string LoadingTerminal { get; set; } = string.Empty;
        public string BerthNumber { get; set; } = string.Empty;

        public int InspectionId { get; set; }        
    }
}
