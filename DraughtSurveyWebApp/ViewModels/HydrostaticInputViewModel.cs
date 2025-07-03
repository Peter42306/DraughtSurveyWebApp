using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.ViewModels
{
    public class HydrostaticInputViewModel
    {
        public int InspectionId { get; set; }
        public int DraughtSurveyBlockId { get; set; }

        //===================== Data input ============================

        // ==================== Table draughts ========================
        //[Display(Name = "Table below")]
        public double? DraughtAbove { get; set; }

        //[Display(Name = "Table above")]
        public double? DraughtBelow { get; set; }

        //[Display(Name = "Mean Draught")]
        public double? MeanAdjustedDraught { get; set; }


        //[Display(Name = "+50 cm")]
        public double? DraughtAboveMTCPlus50 { get; set; }

        //[Display(Name = "-50 cm")]
        public double? DraughtAboveMTCMinus50 { get; set; }

        //[Display(Name = "+50 cm")]
        public double? DraughtBelowMTCPlus50 { get; set; }

        //[Display(Name = "-50 cm")]
        public double? DraughtBelowMTCMinus50 { get; set; }


        // ==================== Values for "above" ====================
        public double? DisplacementAbove { get; set; }
        public double? TPCAbove { get; set; }
        public double? LCFAbove { get; set; }
        public double? MTCPlus50Above { get; set; }
        public double? MTCMinus50Above { get; set; }
        public double? LCFfromAftAbove { get; set; }

        // ==================== Values for "below" ====================
        public double? DisplacementBelow { get; set; }
        public double? TPCBelow { get; set; }
        public double? LCFBelow { get; set; }
        public double? MTCPlus50Below { get; set; }
        public double? MTCMinus50Below { get; set; }
        public double? LCFfromAftBelow { get; set; }



        // ==================== Results ===============================

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
    }
}
