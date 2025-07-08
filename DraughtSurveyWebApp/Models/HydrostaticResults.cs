using System.ComponentModel.DataAnnotations.Schema;

namespace DraughtSurveyWebApp.Models
{
    public class HydrostaticResults
    {
        public int Id { get; set; }

        public double? DisplacementFromTable { get; set; }
        public double? TPCFromTable { get; set; }        
        public double? LCFFromTable { get; set; } 
        public double? MTCPlus50FromTable { get; set; }
        public double? MTCMinus50FromTable { get; set; }

        public double? FirstTrimCorrection { get; set; }
        public double? SecondTrimCorrection { get; set; }
        public double? DisplacementCorrectedForTrim { get; set; }
        public double? DisplacementCorrectedForDensity { get; set; }
        public double? NettoDisplacement { get; set; }
        public double? CargoPlusConstant { get; set; }

        public int DraughtSurveyBlockId { get; set; }
        public required DraughtSurveyBlock DraughtSurveyBlock { get; set; }
    }
}
