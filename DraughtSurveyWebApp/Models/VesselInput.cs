using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.Models
{
    public class VesselInput
    {
        public int Id { get; set; }                

        public string IMO {  get; set; } = string.Empty;        
        public double? LBP { get; set; } // Length Between Perpendiculars
        public double? BM { get; set; } // Breadth Moulded        
        public double? LS { get; set; } // Light Ship                
        public double? SDWT { get; set; } // Summer Deadweight
        public double? DeclaredConstant { get; set; } // Constant declcared by the vessel                                

        public List<UserHydrostaticTableRow> UserHydrostaticTableRows { get; set; } = new List<UserHydrostaticTableRow>();


        public int InspectionId { get; set; }
        public required Inspection Inspection { get; set; }
    }
}
