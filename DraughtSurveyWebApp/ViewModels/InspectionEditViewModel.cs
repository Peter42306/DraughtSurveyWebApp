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

        [Required]
        public string Port { get; set; } = string.Empty;

        [Required]
        public string CompanyReference { get; set; } = string.Empty;

        [Required]
        public OperationType OperationType { get; set; }
    }
}
