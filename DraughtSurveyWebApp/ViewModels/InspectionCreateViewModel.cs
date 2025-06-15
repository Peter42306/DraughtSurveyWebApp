using DraughtSurveyWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.ViewModels
{
    public class InspectionCreateViewModel
    {
        [Required]
        public string VesselName { get; set; } = string.Empty;

        [Required]
        public string Port { get; set; } = string.Empty;

        [Required]
        public string CompanyReference { get; set; } = string.Empty;

        [Required]
        public OperationType OperationType { get; set; }
    }
}
