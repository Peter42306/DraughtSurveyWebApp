using DraughtSurveyWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.ViewModels
{
    public class CargoInputViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Cargo")]
        public string? CargoName { get; set; }

        [Display(Name = "Declared Weight")]
        public double? DeclaredWeight { get; set; }

        [Display(Name = "Terminal")]
        public string? LoadingTerminal { get; set; }

        [Display(Name = "Berth No.")]
        public string? BerthNumber { get; set; }
                
        public string? Shipper { get; set; }
        public string? Consignee { get; set; }                
        public string? DischargingPort { get; set; }

        public int InspectionId { get; set; }        
        public OperationType? OperationType { get; set; }
    }
}
