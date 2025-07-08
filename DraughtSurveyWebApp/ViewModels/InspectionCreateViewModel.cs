using DraughtSurveyWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.ViewModels
{
    public class InspectionCreateViewModel
    {
        [Required]
        public string VesselName { get; set; } = string.Empty;
                
        public string? Port { get; set; }
        
        public string? CompanyReference { get; set; }
                
        public OperationType? OperationType { get; set; }
    }
}
