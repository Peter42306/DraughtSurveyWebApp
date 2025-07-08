using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.Services;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DraughtSurveyWebApp.Controllers
{
    public class HydrostaticInputController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SurveyCalculationsService _surveyCalculationsService;

        public HydrostaticInputController(ApplicationDbContext context, SurveyCalculationsService surveyCalculationsService)
        {
            _context = context;
            _surveyCalculationsService = surveyCalculationsService;
        }

        // GET: HydrostaticInput/Edit?draughtSurveyBlockId=3
        public async Task<IActionResult> Edit(int draughtSurveyBlockId)
        {
            var draughtSurveyBlock = await _context.DraughtSurveyBlocks
                .Include(h => h.HydrostaticInput)
                .Include(r => r.DraughtsResults)
                .Include(r => r.HydrostaticResults)
                .FirstOrDefaultAsync(b => b.Id == draughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }

            var inputs = draughtSurveyBlock.HydrostaticInput;
            var resultsDraughts = draughtSurveyBlock.DraughtsResults;
            var resultsHydrostatic = draughtSurveyBlock.HydrostaticResults;


            double? draughtAboveMTCPlus50 = null;
            double? draughtBelowMTCPlus50 = null;
            double? draughtAboveMTCMinus50 = null;
            double? draughtBelowMTCMinus50 = null;

            double? draughtAbove = inputs?.DraughtAbove;
            double? draughtBelow = inputs?.DraughtBelow;


            if (draughtAbove.HasValue && draughtBelow.HasValue)
            {
                draughtAboveMTCPlus50 = _surveyCalculationsService.CalculateDraughtForMTCPlus50(draughtAbove.Value);
                draughtAboveMTCMinus50 = _surveyCalculationsService.CalculateDraughtForMTCMinus50(draughtAbove.Value);
                draughtBelowMTCPlus50 = _surveyCalculationsService.CalculateDraughtForMTCPlus50(draughtBelow.Value);
                draughtBelowMTCMinus50 = _surveyCalculationsService.CalculateDraughtForMTCMinus50(draughtBelow.Value);
            }



            var viewModel = new HydrostaticInputViewModel
            {
                DraughtSurveyBlockId = draughtSurveyBlock.Id,
                InspectionId = draughtSurveyBlock.InspectionId,

                DraughtBelow = inputs?.DraughtBelow,
                DraughtAbove = inputs?.DraughtAbove,
                MeanAdjustedDraught = resultsDraughts?.MeanAdjustedDraught,
                MeanAdjustedDraughtAfterKeelCorrection = resultsDraughts?.MeanAdjustedDraughtAfterKeelCorrection,

                DisplacementAbove = inputs?.DisplacementAbove,
                DisplacementBelow = inputs?.DisplacementBelow,
                DisplacementFromTable = resultsHydrostatic?.DisplacementFromTable,

                TPCAbove = inputs?.TPCAbove,
                TPCBelow = inputs?.TPCBelow,
                TPCFromTable = resultsHydrostatic?.TPCFromTable,

                LCFAbove = inputs?.LCFAbove,
                LCFBelow = inputs?.LCFBelow,
                LCFFromTable = resultsHydrostatic?.LCFFromTable,
                IsLCFForward = inputs?.IsLCFForward,


                DraughtAboveMTCPlus50 = draughtAboveMTCPlus50,
                DraughtAboveMTCMinus50 = draughtAboveMTCMinus50,
                DraughtBelowMTCPlus50 = draughtBelowMTCPlus50,
                DraughtBelowMTCMinus50 = draughtBelowMTCMinus50,

                MTCPlus50Above = inputs?.MTCPlus50Above,
                MTCPlus50Below = inputs?.MTCPlus50Below,
                MTCMinus50Above = inputs?.MTCMinus50Above,
                MTCMinus50Below = inputs?.MTCMinus50Below,

                MTCPlus50FromTable = resultsHydrostatic?.MTCPlus50FromTable,
                MTCMinus50FromTable = resultsHydrostatic?.MTCMinus50FromTable,

                FirstTrimCorrection = resultsHydrostatic?.FirstTrimCorrection,
                SecondTrimCorrection = resultsHydrostatic?.SecondTrimCorrection,
                DisplacementCorrectedForTrim = resultsHydrostatic?.DisplacementCorrectedForTrim,
                DisplacementCorrectedForDensity = resultsHydrostatic?.DisplacementCorrectedForDensity,
                NettoDisplacement = resultsHydrostatic?.NettoDisplacement,
                CargoPlusConstant = resultsHydrostatic?.CargoPlusConstant
            };

            return View(viewModel);
        }


        // POST: 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HydrostaticInputViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }


            var draughtSurveyBlock = await _context.DraughtSurveyBlocks
                .Include(b => b.DraughtsInput)
                .Include(r => r.DraughtsResults)
                .Include(h => h.HydrostaticInput)
                .Include(r => r.HydrostaticResults)
                .Include(r => r.DeductiblesResults)
                .Include(i => i.Inspection)
                    .ThenInclude(i => i.VesselInput)
                .FirstOrDefaultAsync(b => b.Id == viewModel.DraughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }


            var resultsDraughts = draughtSurveyBlock.DraughtsResults;

            var input = draughtSurveyBlock.HydrostaticInput;
            var results = draughtSurveyBlock.HydrostaticResults;

            if (input == null)
            {
                input = new Models.HydrostaticInput
                {
                    DraughtSurveyBlockId = viewModel.DraughtSurveyBlockId,
                    DraughtSurveyBlock = draughtSurveyBlock
                };

                draughtSurveyBlock.HydrostaticInput = input;
                _context.HydrostaticInputs.Add(draughtSurveyBlock.HydrostaticInput);
            }            

            if (results == null)
            {
                results = new Models.HydrostaticResults
                {
                    DraughtSurveyBlockId = draughtSurveyBlock.Id,
                    DraughtSurveyBlock = draughtSurveyBlock
                };

                draughtSurveyBlock.HydrostaticResults = results;
                _context.HydrostaticResults.Add(results);
            }


            input.DraughtAbove = viewModel.DraughtAbove;
            input.DraughtBelow = viewModel.DraughtBelow;
            input.DisplacementAbove = viewModel.DisplacementAbove;
            input.DisplacementBelow = viewModel.DisplacementBelow;
            input.TPCAbove = viewModel.TPCAbove;
            input.TPCBelow = viewModel.TPCBelow;
            input.LCFAbove = viewModel.LCFAbove;
            input.LCFBelow = viewModel.LCFBelow;
            input.IsLCFForward = viewModel.IsLCFForward;

            input.MTCPlus50Above = viewModel.MTCPlus50Above;
            input.MTCPlus50Below = viewModel.MTCPlus50Below;
            input.MTCMinus50Above = viewModel.MTCMinus50Above;
            input.MTCMinus50Below = viewModel.MTCMinus50Below;



            double? displacementFromTable = null;
            double? tPCFromTable = null;
            double? lCFFromTable = null;

            double? meanAdjustedDraughtAfterKeelCorrection = resultsDraughts?.MeanAdjustedDraughtAfterKeelCorrection;

            double? draughtAbove = input.DraughtAbove;
            double? draughtBelow = input.DraughtBelow;

            double? displacementAbove = input.DisplacementAbove;            
            double? displacementBelow = input.DisplacementBelow;
            double? tpcAbove = input.TPCAbove;
            double? tpcBelow = input.TPCBelow;
            double? lcfAbove = input.LCFAbove;
            double? lcfBelow = input.LCFBelow;


            if (!meanAdjustedDraughtAfterKeelCorrection.HasValue)
            {
                meanAdjustedDraughtAfterKeelCorrection = 0.0;
            }

            if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                draughtAbove.HasValue &&
                displacementAbove.HasValue &&
                draughtBelow.HasValue &&
                displacementBelow.HasValue)
            {
                displacementFromTable = _surveyCalculationsService.CalculateLinearInterpolation(
                    draughtAbove.Value,
                    displacementAbove.Value,
                    draughtBelow.Value,
                    displacementBelow.Value,
                    meanAdjustedDraughtAfterKeelCorrection.Value);
            }


            if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                draughtAbove.HasValue &&
                tpcAbove.HasValue &&
                draughtBelow.HasValue &&
                tpcBelow.HasValue)
            {
                tPCFromTable = _surveyCalculationsService.CalculateLinearInterpolation(
                    draughtAbove.Value,
                    tpcAbove.Value,
                    draughtBelow.Value,
                    tpcBelow.Value,
                    meanAdjustedDraughtAfterKeelCorrection.Value);
            }


            if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                draughtAbove.HasValue &&
                lcfAbove.HasValue &&
                draughtBelow.HasValue &&
                lcfBelow.HasValue)
            {
                lCFFromTable = _surveyCalculationsService.CalculateLinearInterpolation(
                    draughtAbove.Value,
                    lcfAbove.Value,
                    draughtBelow.Value,
                    lcfBelow.Value,
                    meanAdjustedDraughtAfterKeelCorrection.Value);
            }



            // Calculation of draughts +/- 50 cm, above and below 
            double? mTCPlus50FromTable = null;
            double? mTCMinus50FromTable = null;
            double? mtcPlus50Above = input.MTCPlus50Above;
            double? mtcPlus50Below = input.MTCPlus50Below;
            double? mtcMinus50Above = input.MTCMinus50Above;
            double? mtcMinus50Below = input.MTCMinus50Below;

            if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                draughtAbove.HasValue &&
                mtcPlus50Above.HasValue &&
                draughtBelow.HasValue &&
                mtcPlus50Below.HasValue
                )
            {
                mTCPlus50FromTable = _surveyCalculationsService.CalculateLinearInterpolation(
                    draughtAbove.Value + 0.5,
                    mtcPlus50Above.Value,
                    draughtBelow.Value + 0.5,
                    mtcPlus50Below.Value,
                    meanAdjustedDraughtAfterKeelCorrection.Value + 0.5
                    );                
            }

            if (meanAdjustedDraughtAfterKeelCorrection.HasValue &&
                draughtAbove.HasValue &&
                mtcMinus50Above.HasValue &&
                draughtBelow.HasValue &&
                mtcMinus50Below.HasValue)
            {
                mTCMinus50FromTable = _surveyCalculationsService.CalculateLinearInterpolation(
                    draughtAbove.Value - 0.5,
                    mtcMinus50Above.Value,
                    draughtBelow.Value - 0.5,
                    mtcMinus50Below.Value,
                    meanAdjustedDraughtAfterKeelCorrection.Value - 0.5
                    );
            }

            

            // Calculation of the 1st trim corrrection            
            double? firstTrimCorrection = null;

            double? trimCorrected = draughtSurveyBlock.DraughtsResults?.TrimCorrected;
            double? lcf = draughtSurveyBlock.HydrostaticResults?.LCFFromTable;
            bool? isLCFForward = draughtSurveyBlock.HydrostaticInput?.IsLCFForward;
            double? tpc = draughtSurveyBlock.HydrostaticResults?.TPCFromTable;
            double? lbp = draughtSurveyBlock.Inspection?.VesselInput?.LBP;

            if (trimCorrected.HasValue &&
                lcf.HasValue &&
                isLCFForward.HasValue &&
                tpc.HasValue &&
                lbp.HasValue)
            {
                firstTrimCorrection = _surveyCalculationsService.CalculateFirstTrimCorrection(
                    trimCorrected.Value,
                    lcf.Value,
                    isLCFForward.Value,
                    tpc.Value,
                    lbp.Value
                    );
            }



            // Calculcation of the 2nd trim correction

            double? secondTrimCorrection = null;

            double? mtcMinus50 = draughtSurveyBlock.HydrostaticResults?.MTCMinus50FromTable;
            double? mtcPlus50 = draughtSurveyBlock.HydrostaticResults?.MTCPlus50FromTable;

            if (trimCorrected.HasValue &&
                mtcMinus50.HasValue &&
                mtcPlus50.HasValue &&
                lbp.HasValue)
            {
                secondTrimCorrection = _surveyCalculationsService.CalculateSecondTrimCorrection(
                    trimCorrected.Value,
                    mtcMinus50.Value,
                    mtcPlus50.Value,
                    lbp.Value
                    );
            }



            // Calculation of Displacement corrected for trim corrections

            double? displacementCorrectedForTrim = null;

            if (displacementFromTable.HasValue &&
                firstTrimCorrection.HasValue &&
                secondTrimCorrection.HasValue)
            {
                displacementCorrectedForTrim = _surveyCalculationsService.CalculateDisplacementCorrectedForTrim(
                    displacementFromTable.Value,
                    firstTrimCorrection.Value,
                    secondTrimCorrection.Value
                    );
            }


            // Calculation of Displacement corrected for density

            double? displacementCorrectedForDensity = null;
            double? seaWaterDensity = draughtSurveyBlock.DraughtsInput?.SeaWaterDensity;

            if (displacementCorrectedForTrim.HasValue &&
                seaWaterDensity.HasValue)
            {
                displacementCorrectedForDensity = _surveyCalculationsService.CalculateDisplacementCorrectedForDensity(
                    displacementCorrectedForTrim.Value,
                    seaWaterDensity.Value
                    );
            }



            // Calculation of NETTO displacement

            double? totalDeductibles = draughtSurveyBlock.DeductiblesResults?.TotalDeductibles;
            double? nettoDisplacement = null;

            if (displacementCorrectedForDensity.HasValue &&
                totalDeductibles.HasValue)
            {
                nettoDisplacement = _surveyCalculationsService.CalculateNettoDisplacement(
                    displacementCorrectedForDensity.Value,
                    totalDeductibles.Value
                    );
            }



            // Calculation of Cargo + Constant

            double? cargoPlusConstant = null;
            double? lightShip = draughtSurveyBlock.Inspection?.VesselInput?.LS;

            if (nettoDisplacement.HasValue &&
                lightShip.HasValue
                )
            {
                cargoPlusConstant = _surveyCalculationsService.CalculateCargoPlusConstant(
                    nettoDisplacement.Value,
                    lightShip.Value
                    );
            }



            

            

            if (displacementFromTable.HasValue)
            {
                results.DisplacementFromTable = displacementFromTable.Value;
            }

            if (tPCFromTable.HasValue)
            {
                results.TPCFromTable = tPCFromTable.Value;
            }

            if (lCFFromTable.HasValue)
            {
                results.LCFFromTable = lCFFromTable.Value;
            }

            if (mTCPlus50FromTable.HasValue)
            {
                results.MTCPlus50FromTable = mTCPlus50FromTable.Value;
            }

            if (mTCMinus50FromTable.HasValue)
            {
                results.MTCMinus50FromTable = mTCMinus50FromTable.Value;
            }

            if (firstTrimCorrection.HasValue)
            {
                results.FirstTrimCorrection = firstTrimCorrection.Value;
            }

            if (secondTrimCorrection.HasValue)
            {
                results.SecondTrimCorrection = secondTrimCorrection.Value;
            }

            if (displacementCorrectedForTrim.HasValue)
            {
                results.DisplacementCorrectedForTrim = displacementCorrectedForTrim.Value;
            }

            if (displacementCorrectedForDensity.HasValue)
            {
                results.DisplacementCorrectedForDensity = displacementCorrectedForDensity.Value;
            }

            if (nettoDisplacement.HasValue)
            {
                results.NettoDisplacement = nettoDisplacement.Value;
            }

            if (cargoPlusConstant.HasValue)
            {
                results.CargoPlusConstant = cargoPlusConstant.Value;
            }



            await _context.SaveChangesAsync();

            //return RedirectToAction("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId });
            return Redirect($"{Url.Action("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId })}#initial-draught-hydrostatics");
        }
    }
}
