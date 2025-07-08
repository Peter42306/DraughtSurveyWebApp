namespace DraughtSurveyWebApp.Models
{
    public class DeductiblesInput
    {

        public int Id { get; set; }        

        public double? Ballast { get; set; }
        public double? FreshWater { get; set; }
        public double? FuelOil { get; set; }
        public double? DieselOil { get; set; }
        public double? LubOil { get; set; }
        public double? Others { get; set; }

        public int DraughtSurveyBlockId { get; set; }
        public required DraughtSurveyBlock DraughtSurveyBlock { get; set; }
    }
}
