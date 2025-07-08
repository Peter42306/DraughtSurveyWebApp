using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.ViewModels
{
    public class HydrostaticInputViewModel
    {
        public int InspectionId { get; set; }
        public int DraughtSurveyBlockId { get; set; }

        //===================== Data input ============================

        // ==================== Table draughts ========================
        [Display(Name = "Table draught")]
        public double? DraughtAbove { get; set; }

        [Display(Name = "Table draught")]
        public double? DraughtBelow { get; set; }

        [Display(Name = "Mean Draught")]
        public double? MeanAdjustedDraught { get; set; }

        [Display(Name = "Mean Draught")]
        public double? MeanAdjustedDraughtAfterKeelCorrection { get; set; }


        [Display(Name = "+50 cm")]
        public double? DraughtAboveMTCPlus50 { get; set; }

        [Display(Name = "-50 cm")]
        public double? DraughtAboveMTCMinus50 { get; set; }

        [Display(Name = "+50 cm")]
        public double? DraughtBelowMTCPlus50 { get; set; }

        [Display(Name = "-50 cm")]
        public double? DraughtBelowMTCMinus50 { get; set; }        


        public bool? IsLCFForward { get; set; }


        // ==================== Values for "above" ====================
        [Display(Name = "Table D")]
        public double? DisplacementAbove { get; set; }

        [Display(Name = "Table TPC")]
        public double? TPCAbove { get; set; }

        [Display(Name = "Table LCF")]
        public double? LCFAbove { get; set; }

        [Display(Name = "Table +50 cm")]
        public double? MTCPlus50Above { get; set; }

        [Display(Name = "Table -50 cm")]
        public double? MTCMinus50Above { get; set; }
        public double? LCFfromAftAbove { get; set; }

        // ==================== Values for "below" ====================
        [Display(Name = "Table D")]
        public double? DisplacementBelow { get; set; }

        [Display(Name = "Table TPC")]
        public double? TPCBelow { get; set; }

        [Display(Name = "Table LCF")]
        public double? LCFBelow { get; set; }

        [Display(Name = "Table +50 cm")]
        public double? MTCPlus50Below { get; set; }

        [Display(Name = "Table -50 cm")]
        public double? MTCMinus50Below { get; set; }

        public double? LCFfromAftBelow { get; set; }



        // ==================== Results ===============================

        [Display(Name = "Table D")]
        public double? DisplacementFromTable { get; set; }

        [Display(Name = "Table TPC")]
        public double? TPCFromTable { get; set; }

        [Display(Name = "Table LCF")]
        public double? LCFFromTable { get; set; }

        [Display(Name = "Table +50 cm")]
        public double? MTCPlus50FromTable { get; set; }

        [Display(Name = "Table -50 cm")]
        public double? MTCMinus50FromTable { get; set; }

        [Display(Name = "1st Correction")]
        public double? FirstTrimCorrection { get; set; }

        [Display(Name = "2nd Correction")]
        public double? SecondTrimCorrection { get; set; }

        [Display(Name = "D corrected for Trim")]
        public double? DisplacementCorrectedForTrim { get; set; }

        [Display(Name = "D corrected for Density")]
        public double? DisplacementCorrectedForDensity { get; set; }

        [Display(Name = "NETTO Displacement")]
        public double? NettoDisplacement { get; set; }

        [Display(Name = "Cargo + Constant")]
        public double? CargoPlusConstant { get; set; }        
    }
}
