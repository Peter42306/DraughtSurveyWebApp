using DraughtSurveyWebApp.Models;

namespace DraughtSurveyWebApp.Services
{
    public static class ExcelExportMapper
    {
        public static Dictionary<string, object?> CreateMap(
            Inspection inspection,
            DraughtSurveyBlock initialDraughtSurveyBlock,
            DraughtSurveyBlock finalDraughtSurveyBlock)
        {
            return new Dictionary<string, object?>
            {
                ["Vessel"] = inspection.VesselName,
                ["CompanyReference"] = inspection.CompanyReference,
                ["CargoByDraughtSurvey"] = inspection?.CargoResult?.CargoByDraughtSurvey
            };
        }
    }
}
