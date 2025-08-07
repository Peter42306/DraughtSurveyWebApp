using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.Models
{
    public class UserHydrostaticTableRow
    {
        public int Id { get; set; }

        [Range(0.001, 30, ErrorMessage = "Please enter a valid draught value")]
        public double? Draught { get; set; }

        public double? Displacement { get; set; }
        public double? TPC { get; set; }
        public double? LCF { get; set; }

        [Display(Name = "LCF from Midship")]
        public bool? IsLcfForward { get; set; }

        [Display(Name = "MTC +50")]
        public double? MTCPlus50 { get; set; }

        [Display(Name = "MTC -50")]
        public double? MTCMinus50 { get; set; }

        public int UserHydrostaticTableHeaderId { get; set; }
        public UserHydrostaticTableHeader? UserHydrostaticTableHeader { get; set; }
    }
}


