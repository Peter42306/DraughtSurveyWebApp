using Microsoft.AspNetCore.Mvc.Rendering;

namespace DraughtSurveyWebApp.ViewModels
{
    public class ExcelTemplateUserSelectTemplate
    {
        public int InspectionId { get; set; }
        public int? SelectedTemplateId { get; set; }
        public IEnumerable<SelectListItem>ExcelTemplates { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
    