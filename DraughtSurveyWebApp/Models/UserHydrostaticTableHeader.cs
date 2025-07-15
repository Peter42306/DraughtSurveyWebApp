namespace DraughtSurveyWebApp.Models
{
    public class UserHydrostaticTableHeader
    {
        public int Id { get; set; }

        public string IMO { get; set; } = string.Empty;
        public string VesselName { get; set; } = string.Empty;

        public double? TableStep { get; set; }

        public required string ApplicationUserId { get; set; }
        public required ApplicationUser ApplicationUser { get; set; }

        public List<UserHydrostaticTableRow> UserHydrostaticTableRows { get; set; } = new List<UserHydrostaticTableRow>();
    }
}
