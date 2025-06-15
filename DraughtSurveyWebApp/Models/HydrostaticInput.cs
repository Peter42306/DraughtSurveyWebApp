namespace DraughtSurveyWebApp.Models
{
    public class HydrostaticInput
    {
        public int Id { get; set; }

        // Table draughts
        public double DraughtAbove { get; set; }
        public double DraughtBelow { get; set; }

        // Values for "above"
        public double DisplacementAbove { get; set; }
        public double TPCAbove { get; set; }
        public double LCFAbove { get; set; }
        public double MTCPlus50Above { get; set; }
        public double MTCMinus50Above { get; set; }
        public double? LCFfromAftAbove { get; set; }

        // Values for "below"
        public double DisplacementBelow { get; set; }
        public double TPCBelow { get; set; }
        public double LCFBelow { get; set; }
        public double MTCPlus50Below { get; set; }
        public double MTCMinus50Below { get; set; }
        public double? LCFfromAftBelow { get; set; }

        public int DraughtSurveyBlockId { get; set; }
        public required DraughtSurveyBlock DraughtSurveyBlock { get; set; }
    }
}
