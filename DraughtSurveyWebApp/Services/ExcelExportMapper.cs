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
                // General info
                ["CompanyReference"] = string.IsNullOrWhiteSpace(inspection.CompanyReference)
                    ? "DRAFT SURVEY REPORT"
                    : $"DRAFT SURVEY REPORT No. {inspection.CompanyReference}",
                ["Vessel"] = inspection.VesselName,
                ["CargoName"] = inspection.CargoInput?.CargoName,
                ["DeclaredWeight"] = inspection.CargoInput?.DeclaredWeight,
                ["Shipper"] = string.IsNullOrWhiteSpace(inspection.CargoInput?.Shipper)
                    ? "-"
                    : inspection.CargoInput?.Shipper,                
                ["Consignee"] = string.IsNullOrWhiteSpace(inspection.CargoInput?.Consignee) 
                    ? "-" 
                    : inspection.CargoInput?.Consignee,                

                ["Port"] = inspection.Port,
                ["PresentPort"] = inspection.OperationType == OperationType.Loading 
                    ? "Load Port"
                    : "Discharge Port",
                ["DischargingPort"] = inspection.CargoInput?.DischargingPort,
                ["LoadingOrDestinationPort"] = inspection.OperationType == OperationType.Loading
                    ? "Discharge Port"
                    : "Load Port",

                ["LS"] = inspection.VesselInput?.LS,
                ["LBP"] = inspection.VesselInput?.LBP,                                
                ["CargoByDraughtSurvey"] = inspection.CargoResult?.CargoByDraughtSurvey != null 
                    ? $"TOTAL WEIGHT OF CARGO   {inspection.CargoResult?.CargoByDraughtSurvey:0.000}   MTS"
                    : $"TOTAL WEIGHT OF CARGO - MTS",         

                // Initial draughts
                ["DraughtFwdPS_Initial"] = initialDraughtSurveyBlock.DraughtsInput?.DraughtFwdPS,
                ["DraughtFwdSS_Initial"] = initialDraughtSurveyBlock.DraughtsInput?.DraughtFwdSS,
                ["DraughtMeanFwd_Initial"] = initialDraughtSurveyBlock.DraughtsResults?.DraughtMeanFwd,
                ["DraughtCorrectionFwd_Initial"] = initialDraughtSurveyBlock.DraughtsResults?.DraughtCorrectedFwd,
                ["DraughtCorrectedFwd_Initial"] = initialDraughtSurveyBlock.DraughtsResults?.DraughtCorrectedFwd,

                ["DraughtMidPS_Initial"] = initialDraughtSurveyBlock.DraughtsInput?.DraughtMidPS,
                ["DraughtMidSS_Initial"] = initialDraughtSurveyBlock.DraughtsInput?.DraughtMidSS,
                ["DraughtMeanMid_Initial"] = initialDraughtSurveyBlock.DraughtsResults?.DraughtMeanMid,
                ["DraughtCorrectionMid_Initial"] = initialDraughtSurveyBlock.DraughtsResults?.DraughtCorrectionMid,
                ["DraughtCorrectedMid_Initial"] = initialDraughtSurveyBlock.DraughtsResults?.DraughtCorrectedMid,

                ["DraughtAftPS_Initial"] = initialDraughtSurveyBlock.DraughtsInput?.DraughtAftPS,
                ["DraughtAftSS_Initial"] = initialDraughtSurveyBlock.DraughtsInput?.DraughtAftSS,
                ["DraughtMeanAft_Initial"] = initialDraughtSurveyBlock.DraughtsResults?.DraughtMeanAft,
                ["DraughtCorrectionAft_Initial"] = initialDraughtSurveyBlock.DraughtsResults?.DraughtCorrectionAft,
                ["DraughtCorrectedAft_Initial"] = initialDraughtSurveyBlock.DraughtsResults?.DraughtCorrectedAft,

                ["DistanceFwd_Initial"] = initialDraughtSurveyBlock.DraughtsInput?.DistanceFwd,
                ["DistanceAft_Initial"] = initialDraughtSurveyBlock.DraughtsInput?.DistanceAft,
                ["DistanceMid_Initial"] = initialDraughtSurveyBlock.DraughtsInput?.DistanceMid,

                ["LBD_Initial"] = initialDraughtSurveyBlock.DraughtsResults?.LBD,
                ["TrimApparent_Initial"] = initialDraughtSurveyBlock.DraughtsResults?.TrimApparent,
                ["TrimCorrected_Initial"] = initialDraughtSurveyBlock.DraughtsResults?.TrimCorrected,

                ["SeaWaterDensity_Initial"] = initialDraughtSurveyBlock.DraughtsInput?.SeaWaterDensity,
                ["Swell_Initial"] = initialDraughtSurveyBlock.DraughtsInput?.Swell,

                ["MeanAdjustedDraught_Initial"] = initialDraughtSurveyBlock.DraughtsResults?.MeanAdjustedDraught,

                // Initial Hydrostatics
                ["DisplacementFromTable_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.DisplacementFromTable,
                ["FirstTrimCorrection_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.FirstTrimCorrection,
                ["SecondTrimCorrection_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.SecondTrimCorrection,
                ["DisplacementCorrectedForTrim_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.DisplacementCorrectedForTrim,
                ["DisplacementCorrectedForDensity_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.DisplacementCorrectedForDensity,
                ["TotalDeductibles1_Initial"] = initialDraughtSurveyBlock.DeductiblesResults?.TotalDeductibles,
                ["NettoDisplacement_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.NettoDisplacement,
                ["CargoPlusConstant_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.CargoPlusConstant,

                ["LCFFromTable_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.LCFFromTable,
                ["TPCFromTable_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.TPCFromTable,
                ["MTCPlus50FromTable_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.MTCPlus50FromTable,
                ["MTCMinus50FromTable_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.MTCMinus50FromTable,

                // Initial Deductibles
                ["TotalDeductibles_Initial"] = initialDraughtSurveyBlock.DeductiblesResults?.TotalDeductibles,
                ["FuelOil_Initial"] = initialDraughtSurveyBlock.DeductiblesInput?.FuelOil,
                ["DieselOil_Initial"] = initialDraughtSurveyBlock.DeductiblesInput?.DieselOil,
                ["LubOil_Initial"] = initialDraughtSurveyBlock.DeductiblesInput?.LubOil,
                ["Ballast_Initial"] = initialDraughtSurveyBlock.DeductiblesInput?.Ballast,
                ["FreshWater_Initial"] = initialDraughtSurveyBlock.DeductiblesInput?.FreshWater,
                ["Others_Initial"] = initialDraughtSurveyBlock.DeductiblesInput?.Others,

                // Initial dates & times
                ["SurveyDate_Initial"] = initialDraughtSurveyBlock.SurveyTimeStart?.ToString("dd.MM.yyyy"),
                ["SurveyTimeRange_Initial"] = initialDraughtSurveyBlock.SurveyTimeStart.HasValue && initialDraughtSurveyBlock.SurveyTimeEnd.HasValue 
                    ? $"{initialDraughtSurveyBlock.SurveyTimeStart:HH\\:mm}-{initialDraughtSurveyBlock.SurveyTimeEnd:HH\\:mm}" 
                    : string.Empty,


                // Final draughts
                ["DraughtFwdPS_Final"] = finalDraughtSurveyBlock.DraughtsInput?.DraughtFwdPS,
                ["DraughtFwdSS_Final"] = finalDraughtSurveyBlock.DraughtsInput?.DraughtFwdSS,
                ["DraughtMeanFwd_Final"] = finalDraughtSurveyBlock.DraughtsResults?.DraughtMeanFwd,
                ["DraughtCorrectionFwd_Final"] = finalDraughtSurveyBlock.DraughtsResults?.DraughtCorrectedFwd,
                ["DraughtCorrectedFwd_Final"] = finalDraughtSurveyBlock.DraughtsResults?.DraughtCorrectedFwd,

                ["DraughtMidPS_Final"] = finalDraughtSurveyBlock.DraughtsInput?.DraughtMidPS,
                ["DraughtMidSS_Final"] = finalDraughtSurveyBlock.DraughtsInput?.DraughtMidSS,
                ["DraughtMeanMid_Final"] = finalDraughtSurveyBlock.DraughtsResults?.DraughtMeanMid,
                ["DraughtCorrectionMid_Final"] = finalDraughtSurveyBlock.DraughtsResults?.DraughtCorrectionMid,
                ["DraughtCorrectedMid_Final"] = finalDraughtSurveyBlock.DraughtsResults?.DraughtCorrectedMid,

                ["DraughtAftPS_Final"] = finalDraughtSurveyBlock.DraughtsInput?.DraughtAftPS,
                ["DraughtAftSS_Final"] = finalDraughtSurveyBlock.DraughtsInput?.DraughtAftSS,
                ["DraughtMeanAft_Final"] = finalDraughtSurveyBlock.DraughtsResults?.DraughtMeanAft,
                ["DraughtCorrectionAft_Final"] = finalDraughtSurveyBlock.DraughtsResults?.DraughtCorrectionAft,
                ["DraughtCorrectedAft_Final"] = finalDraughtSurveyBlock.DraughtsResults?.DraughtCorrectedAft,

                ["DistanceFwd_Final"] = finalDraughtSurveyBlock.DraughtsInput?.DistanceFwd,
                ["DistanceAft_Final"] = finalDraughtSurveyBlock.DraughtsInput?.DistanceAft,
                ["DistanceMid_Final"] = finalDraughtSurveyBlock.DraughtsInput?.DistanceMid,

                ["LBD_Final"] = finalDraughtSurveyBlock.DraughtsResults?.LBD,
                ["TrimApparent_Final"] = finalDraughtSurveyBlock.DraughtsResults?.TrimApparent,
                ["TrimCorrected_Final"] = finalDraughtSurveyBlock.DraughtsResults?.TrimCorrected,

                ["SeaWaterDensity_Final"] = finalDraughtSurveyBlock.DraughtsInput?.SeaWaterDensity,
                ["Swell_Final"] = finalDraughtSurveyBlock.DraughtsInput?.Swell,

                ["MeanAdjustedDraught_Final"] = finalDraughtSurveyBlock.DraughtsResults?.MeanAdjustedDraught,

                // Final Hydrostatics
                ["DisplacementFromTable_Final"] = finalDraughtSurveyBlock.HydrostaticResults?.DisplacementFromTable,
                ["FirstTrimCorrection_Final"] = finalDraughtSurveyBlock.HydrostaticResults?.FirstTrimCorrection,
                ["SecondTrimCorrection_Final"] = finalDraughtSurveyBlock.HydrostaticResults?.SecondTrimCorrection,
                ["DisplacementCorrectedForTrim_Final"] = finalDraughtSurveyBlock.HydrostaticResults?.DisplacementCorrectedForTrim,
                ["DisplacementCorrectedForDensity_Final"] = finalDraughtSurveyBlock.HydrostaticResults?.DisplacementCorrectedForDensity,
                ["TotalDeductibles1_Final"] = finalDraughtSurveyBlock.DeductiblesResults?.TotalDeductibles,
                ["NettoDisplacement_Final"] = finalDraughtSurveyBlock.HydrostaticResults?.NettoDisplacement,
                ["CargoPlusConstant_Final"] = finalDraughtSurveyBlock.HydrostaticResults?.CargoPlusConstant,

                ["LCFFromTable_Final"] = finalDraughtSurveyBlock.HydrostaticResults?.LCFFromTable,
                ["TPCFromTable_Final"] = finalDraughtSurveyBlock.HydrostaticResults?.TPCFromTable,
                ["MTCPlus50FromTable_Final"] = finalDraughtSurveyBlock.HydrostaticResults?.MTCPlus50FromTable,
                ["MTCMinus50FromTable_Final"] = finalDraughtSurveyBlock.HydrostaticResults?.MTCMinus50FromTable,

                // Final Deductibles
                ["TotalDeductibles_Final"] = finalDraughtSurveyBlock.DeductiblesResults?.TotalDeductibles,
                ["FuelOil_Final"] = finalDraughtSurveyBlock.DeductiblesInput?.FuelOil,
                ["DieselOil_Final"] = finalDraughtSurveyBlock.DeductiblesInput?.DieselOil,
                ["LubOil_Final"] = finalDraughtSurveyBlock.DeductiblesInput?.LubOil,
                ["Ballast_Final"] = finalDraughtSurveyBlock.DeductiblesInput?.Ballast,
                ["FreshWater_Final"] = finalDraughtSurveyBlock.DeductiblesInput?.FreshWater,
                ["Others_Final"] = finalDraughtSurveyBlock.DeductiblesInput?.Others,

                // Final dates & times
                ["SurveyDate_Final"] = finalDraughtSurveyBlock.SurveyTimeStart?.ToString("dd.MM.yyyy"),
                ["SurveyTimeRange_Final"] = finalDraughtSurveyBlock.SurveyTimeStart.HasValue && finalDraughtSurveyBlock.SurveyTimeEnd.HasValue
                    ? $"{finalDraughtSurveyBlock.SurveyTimeStart:HH\\:mm}-{finalDraughtSurveyBlock.SurveyTimeEnd:HH\\:mm}"
                    : string.Empty



            };
        }
    }
}
