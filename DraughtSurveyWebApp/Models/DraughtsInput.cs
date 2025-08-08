namespace DraughtSurveyWebApp.Models
{
    public class DraughtsInput
    {
        public int Id { get; set; }        

        public double? Swell { get; set; }

        // Apparent draughts
        public double? DraughtFwdPS { get; set; }
        public double? DraughtFwdSS { get; set; }
        public double? DraughtMidPS { get; set; }
        public double? DraughtMidSS { get; set; }
        public double? DraughtAftPS { get; set; }
        public double? DraughtAftSS { get; set; }

        // Breadth for estimation of sea side draughts
        public double? BreadthForward { get; set; }
        public double? BreadthAft { get; set; }

        // Distances
        public double? DistanceFwd { get; set; }
        public double? DistanceMid { get; set; }
        public double? DistanceAft { get; set; }

        public bool? isFwdDistancetoFwd { get; set; }
        public bool? isMidDistanceToFwd { get; set; }
        public bool? isAftDistanceToFwd { get; set; }


        public double? SeaWaterDensity { get; set; }
        public double? KeelCorrection { get; set; }

        public int DraughtSurveyBlockId { get; set; }
        public required DraughtSurveyBlock DraughtSurveyBlock { get; set; }
    }
}
