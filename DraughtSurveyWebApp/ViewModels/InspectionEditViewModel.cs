using DraughtSurveyWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.ViewModels
{
    public class InspectionEditViewModel
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        public string VesselName { get; set; } = string.Empty;        
        public string? Port { get; set; }        
        public string? CompanyReference { get; set; }       
        public OperationType? OperationType { get; set; }
        public bool notShowInputWarnings { get; set; }
    }
}
