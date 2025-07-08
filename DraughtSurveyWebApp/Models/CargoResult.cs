namespace DraughtSurveyWebApp.Models
{
    public class CargoResult
    {
        public int Id { get; set; }

        public double? CargoByDraughtSurvey { get; set; }
        public double? DifferenceWithBL_Mt { get; set; }
        public double? DifferenceWithBL_Percents { get; set; }
        public double? DifferenceWithSDWT_Percents { get; set; }

        public int InspectionId { get; set; }
        public Inspection? Inspection { get; set; }
    }
}
