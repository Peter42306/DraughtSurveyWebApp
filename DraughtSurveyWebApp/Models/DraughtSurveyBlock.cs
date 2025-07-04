namespace DraughtSurveyWebApp.Models
{
    public enum SurveyType
    {
        Initial, 
        Final
    }

    public class DraughtSurveyBlock
    {
        public int Id { get; set; }        

        public SurveyType SurveyType { get; set; } // Initial or Final
                
        public DateTime SurveyTimeStart { get; set; }
        public DateTime SurveyTimeEnd { get; set; }
        public DateTime CargoOperationsDateTime { get; set; }

        public DraughtsInput? DraughtsInput { get; set; }
        public HydrostaticInput? HydrostaticInput { get; set; }
        public DeductiblesInput? DeductiblesInput { get; set; }

        public DraughtsResults? DraughtsResults { get; set; }
        public HydrostaticResults? HydrostaticResults { get; set; }        
        public DeductiblesResults? DeductiblesResults { get; set; }

        public string? Notes { get; set; } // for Letter of Reserve details, or any notes

        public int InspectionId { get; set; }
        public Inspection? Inspection { get; set; }
    }
}

