namespace DraughtSurveyWebApp.ViewModels
{
    public class HydrostaticInputViewModel
    {
        public int InspectionId { get; set; }
        public int DraughtSurveyBlockId { get; set; }

        //===================== Data input ============================

        // ==================== Table draughts ========================
        public double? DraughtAbove { get; set; }
        public double? DraughtBelow { get; set; }

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
        public double? FirstTrimCorrection { get; set; }
        public double? SecondTrimCorrection { get; set; }
        public double? DisplacementCorrectedForTrim { get; set; }
        public double? DisplacementCorrectedForDensity { get; set; }
        public double? NettoDisplacement { get; set; }
        public double? CargoPlusConstant { get; set; }        
    }
}
