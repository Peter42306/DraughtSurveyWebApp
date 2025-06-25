using System.ComponentModel.DataAnnotations;

namespace DraughtSurveyWebApp.ViewModels
{
    public class DraughtsInputViewModel
    {
        public int InspectionId { get; set; }
        public int DraughtSurveyBlockId { get; set; }


        //====================== Data input ======================
        
        [Display(Name = "Forward PS")]
        public double? DraughtFwdPS { get; set; }

        [Display(Name = "Forward SS")]
        public double? DraughtFwdSS { get; set; }

        [Display(Name = "Midship PS")]
        public double? DraughtMidPS { get; set; }

        [Display(Name = "Midship SS")]
        public double? DraughtMidSS { get; set; }

        [Display(Name = "Aft PS")]
        public double? DraughtAftPS { get; set; }

        [Display(Name = "Aft SS")]
        public double? DraughtAftSS { get; set; }


        [Display(Name = "Distance Fwd")]
        public double? DistanceFwd { get; set; }
        
        [Display(Name = "Distance Mid")]
        public double? DistanceMid { get; set; }

        [Display(Name = "Distance Aft")]
        public double? DistanceAft { get; set; }


        [Display(Name = "Dock Water Density")]
        [Range(0.99, 1.039, ErrorMessage = "Density must be between 0.990 and 1.039 t/m³")]
        public double? SeaWaterDensity { get; set; }
        
        [Display(Name = "Keel Correction")]
        public double? KeelCorrection { get; set; }


        //================== Results for viewModel ==================
        
        [Display(Name = "Mean Fwd")]
        public double? DraughtMeanFwd { get; set; }

        [Display(Name = "Mean Mid")]
        public double? DraughtMeanMid { get; set; }

        [Display(Name = "Mean Aft")]
        public double? DraughtMeanAft { get; set; }



        [Display(Name = "Apparent Trim")]
        public double? TrimApparent { get; set; }        

        [Display(Name = "Hog/Sag")]
        public double? HoggingSagging { get; set; }

        [Display(Name = "Heel")]
        public double? Heel { get; set; }
        
        [Display(Name = "Corrected Trim")]
        public double? TrimCorrected { get; set; }

        [Display(Name = "Mean Adjusted Draught")]
        public double? MeanAdjustedDraught { get; set; }


        
        [Display(Name = "Correction Fwd")]
        public double? DraughtCorrectionFwd { get; set; }

        [Display(Name = "Correction Mid")]
        public double? DraughtCorrectionMid { get; set; }

        [Display(Name = "Correction Aft")]
        public double? DraughtCorrectionAft { get; set; }



        [Display(Name = "Corrected Fwd")]
        public double? DraughtCorrectedFwd { get; set; }

        [Display(Name = "Corrected Mid")]
        public double? DraughtCorrectedMid { get; set; }

        [Display(Name = "Corrected Aft")]
        public double? DraughtCorrectedAft { get; set; }
    }
}
