namespace DraughtSurveyWebApp.Models
{
    public class DraughtsInput
    {
        public int Id { get; set; }        

        // Apparent draughts
        public double DraughtFwdPS { get; set; }
        public double DraughtFwdSS { get; set; }
        public double DraughtMidPS { get; set; }
        public double DraughtMidSS { get; set; }
        public double DraughtAftPS { get; set; }
        public double DraughtAftSS { get; set; }

        // Distances
        public double DistanceFwd { get; set; }
        public double DistanceMid { get; set; }
        public double DistanceAft { get; set; }

        public double SeaWaterDensity { get; set; }
        public double? KeelCorrection { get; set; }

        public int DraughtSurveyBlockId { get; set; }
        public DraughtSurveyBlock? DraughtSurveyBlock { get; set; }
    }
}
