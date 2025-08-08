using DraughtSurveyWebApp.Models;

namespace DraughtSurveyWebApp.ViewModels
{
    public class ExcelTemplateSelectViewModel
    {
        public int? SelectedTemplateId { get; set; }
        public List<ExcelTemplate> ExcelTemplates { get; set; } = new List<ExcelTemplate>();
        public int InspectionId { get; set; }
    }
}
