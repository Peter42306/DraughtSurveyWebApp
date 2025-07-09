using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.Models
{
    public enum OperationType
    {
        Loading,
        Discharging
    }

    public class Inspection
    {
        public int Id { get; set; }

        public string VesselName { get; set; } = string.Empty;
        public string? Port { get; set; }
        public string? CompanyReference { get; set; }
        public OperationType? OperationType { get; set; } // Loading or discharging

        public string ApplicationUsedId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public VesselInput? VesselInput { get; set; }        
        public CargoInput? CargoInput { get; set; }               

        public List<DraughtSurveyBlock> DraughtSurveyBlocks { get; set; } = new();

        public CargoResult? CargoResult { get; set; }
    }
}
