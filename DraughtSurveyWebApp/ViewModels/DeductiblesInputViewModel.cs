using DraughtSurveyWebApp.Models;

namespace DraughtSurveyWebApp.ViewModels
{
    public class DeductiblesInputViewModel
    {
        public int InspectionId { get; set; }
        public int DraughtSurveyBlockId { get; set; }

        // ==================== Inputs ================================

        public double? Ballast { get; set; }
        public double? FreshWater { get; set; }
        public double? FuelOil { get; set; }
        public double? DieselOil { get; set; }
        public double? LubOil { get; set; }
        public double? Others { get; set; }


        // ==================== Results ===============================

        public double? TotalDeductibles { get; set; }

        public SurveyType SurveyType { get; set; }
    }
}
