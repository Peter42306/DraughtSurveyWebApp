namespace DraughtSurveyWebApp.Models
{
    public class DraughtsResults
    {
        public int Id { get; set; }        

        public double? DraughtMeanFwd { get; set; }
        public double? DraughtMeanMid { get; set; }
        public double? DraughtMeanAft { get; set; }

        public double? DraughtCorrectionFwd { get; set; }
        public double? DraughtCorrectionMid { get; set; }
        public double? DraughtCorrectionAft { get; set; }

        public double? DraughtCorrectedFwd { get; set; }
        public double? DraughtCorrectedMid { get; set; }
        public double? DraughtCorrectedAft { get; set; }

        public double? TrimApparent { get; set; }
        public double? HoggingSagging { get; set; }
        public double? Heel { get; set; }

        public double? TrimCorrected { get; set; }
        public double? MeanAdjustedDraught { get; set; }
        public double? MeanAdjustedDraughtAfterKeelCorrection { get; set; }

        public int DraughtSurveyBlockId { get; set; }
        public required DraughtSurveyBlock DraughtSurveyBlock { get; set; }
    }
}
