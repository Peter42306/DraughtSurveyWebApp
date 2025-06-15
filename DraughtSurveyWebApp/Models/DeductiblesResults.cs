namespace DraughtSurveyWebApp.Models
{
    public class DeductiblesResults
    {
        public int Id { get; set; }        

        public double TotalDeductibles { get; set; }

        public int DraughtSurveyBlockId { get; set; }
        public required DraughtSurveyBlock DraughtSurveyBlock { get; set; }
    }
}
