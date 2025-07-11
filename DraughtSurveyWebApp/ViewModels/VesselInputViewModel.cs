using DraughtSurveyWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.ViewModels
{
    public class VesselInputViewModel
    {
        public int Id { get; set; }

        public string IMO { get; set; } = string.Empty;
        public double? LBP { get; set; }
        public double? BM { get; set; }

        [Display(Name = "Light Ship")]
        public double? LS { get; set; }

        public double? SDWT { get; set; }

        [Display(Name = "Declared Constant")]
        public double? DeclaredConstant { get; set; }        

        public int InspectionId { get; set; }
    }
}
