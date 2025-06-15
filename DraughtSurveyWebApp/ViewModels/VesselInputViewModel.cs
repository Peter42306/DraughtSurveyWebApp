using DraughtSurveyWebApp.Models;

namespace DraughtSurveyWebApp.ViewModels
{
    public class VesselInputViewModel
    {
        public int Id { get; set; }

        public string IMO { get; set; } = string.Empty;
        public double LBP { get; set; }
        public double? BM { get; set; }
        public double LS { get; set; }
        public double? SDWT { get; set; }
        public double? DeclaredConstant { get; set; }

        public int InspectionId { get; set; }
    }
}
