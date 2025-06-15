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
        public string Port { get; set; } = string.Empty;
        public string CompanyReference { get; set; } = string.Empty;
        public OperationType OperationType { get; set; } // Loading or discharging

        public VesselInput? VesselInput { get; set; }        
        public CargoInput? CargoInput { get; set; }               

        public List<DraughtSurveyBlock> DraughtSurveyBlocks { get; set; } = new();

        public CargoResult? CargoResult { get; set; }
    }
}
