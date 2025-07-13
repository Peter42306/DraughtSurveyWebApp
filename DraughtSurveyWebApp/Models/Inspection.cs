using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public string ApplicationUserId { get; set; } = null!;
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string VesselName { get; set; } = string.Empty;
        public string? Port { get; set; }
        public string? CompanyReference { get; set; }
        public OperationType? OperationType { get; set; } // Loading or discharging        

        public VesselInput? VesselInput { get; set; }        
        public CargoInput? CargoInput { get; set; }
        public CargoResult? CargoResult { get; set; }

        public bool notShowInputWarnings { get; set; } = false;

        public List<DraughtSurveyBlock> DraughtSurveyBlocks { get; set; } = new();        
    }
}
