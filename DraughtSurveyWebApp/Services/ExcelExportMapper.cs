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
                ["CompanyReference"] = inspection.CompanyReference,
                ["Vessel"] = inspection.VesselName,
                ["CargoName"] = inspection.CargoInput?.CargoName,
                ["DeclaredWeight"] = inspection.CargoInput?.DeclaredWeight,
                ["Shipper"] = inspection.CargoInput?.Shipper,
                ["Consignee"] = inspection.CargoInput?.Consignee,
                ["DischargingPort"] = inspection.CargoInput?.DischargingPort,

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

                ["MeanAdjustedDraught_Initial"] = initialDraughtSurveyBlock.DraughtsResults?.MeanAdjustedDraught,

                // Initial Hydrostatics
                ["DisplacementFromTable_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.DisplacementFromTable,
                ["FirstTrimCorrection_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.FirstTrimCorrection,
                ["SecondTrimCorrection_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.SecondTrimCorrection,
                ["DisplacementCorrectedForTrim_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.DisplacementCorrectedForTrim,
                ["DisplacementCorrectedForDensity_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.DisplacementCorrectedForDensity,
                ["NettoDisplacement_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.NettoDisplacement,
                ["CargoPlusConstant_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.CargoPlusConstant,

                // Initial Deductibles




                ["DisplacementFromTable_Initial"] = initialDraughtSurveyBlock.HydrostaticResults?.DisplacementFromTable,






                ["DraughtFwdPS_Initial"] = initialDraughtSurveyBlock.DraughtsInput?.DraughtFwdPS,


                ["CargoByDraughtSurvey"] = inspection?.CargoResult?.CargoByDraughtSurvey
            };
        }
    }
}
