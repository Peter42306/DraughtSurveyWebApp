namespace DraughtSurveyWebApp.Models
{
    public class UserHydrostaticTableRow
    {
        public int Id { get; set; }        
        
        public double Draught { get; set; }

        public double? Displacement { get; set; }
        public double? TPC { get; set; }
        public double? LCF { get; set; }
        public bool? IsLcfForward { get; set; }
        public double? MTCPlus50 { get; set; }
        public double? MTCMinus50 { get; set; }

        public int UserHydrostaticTableHeaderId { get; set; }
        public required UserHydrostaticTableHeader UserHydrostaticTableHeader { get; set; } = null!;
    }
}
