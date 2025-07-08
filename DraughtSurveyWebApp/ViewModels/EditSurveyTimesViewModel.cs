using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.ViewModels
{
    public class EditSurveyTimesViewModel
    {
        public int InspectionId { get; set; }
        public int DraughtSurveyBlockId { get; set; }

        [Display(Name = "Survey commenced")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? SurveyTimeStart { get; set; }

        [Display(Name = "Survey completed")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? SurveyTimeEnd { get; set; }

        [Display(Name = "Cargo Operations commenced")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? CargoOperationsDateTime { get; set; }
    }
}
