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

            
            double draughtAboveMTCPlus50 = 0;
            double draughtBelowMTCPlus50 = 0;
            double draughtAboveMTCMinus50 = 0;
            double draughtBelowMTCMinus50 = 0;


            if(inputs?.DraughtAbove != null && inputs?.DraughtBelow != null)
            {
                draughtAboveMTCPlus50 = _surveyCalculationsService.CalculateDraughtForMTCPlus50(inputs.DraughtAbove);
                draughtAboveMTCMinus50 = _surveyCalculationsService.CalculateDraughtForMTCMinus50(inputs.DraughtAbove);
                draughtBelowMTCPlus50 = _surveyCalculationsService.CalculateDraughtForMTCPlus50(inputs.DraughtBelow);                
                draughtBelowMTCMinus50 = _surveyCalculationsService.CalculateDraughtForMTCMinus50(inputs.DraughtBelow);
            }



            var viewModel = new HydrostaticInputViewModel
            {
                DraughtSurveyBlockId = draughtSurveyBlock.Id,
                InspectionId = draughtSurveyBlock.InspectionId,

                DraughtBelow = inputs?.DraughtBelow,
                DraughtAbove = inputs?.DraughtAbove,
                MeanAdjustedDraught = resultsDraughts?.MeanAdjustedDraught,

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

            if (draughtSurveyBlock.HydrostaticInput == null)
            {
                draughtSurveyBlock.HydrostaticInput = new Models.HydrostaticInput
                {
                    DraughtSurveyBlockId = viewModel.DraughtSurveyBlockId,
                    DraughtSurveyBlock = draughtSurveyBlock
                };

                _context.HydrostaticInputs.Add(draughtSurveyBlock.HydrostaticInput);
            }

            var resultsDraughts = draughtSurveyBlock.DraughtsResults;
            var input = draughtSurveyBlock.HydrostaticInput;

            input.DraughtAbove = viewModel.DraughtAbove ?? 0;
            input.DraughtBelow = viewModel.DraughtBelow ?? 0;
            input.DisplacementAbove = viewModel.DisplacementAbove ?? 0;
            input.DisplacementBelow = viewModel.DisplacementBelow ?? 0;
            input.TPCAbove = viewModel.TPCAbove ?? 0;
            input.TPCBelow = viewModel.TPCBelow ?? 0;
            input.LCFAbove = viewModel.LCFAbove ?? 0;
            input.LCFBelow = viewModel.LCFBelow ?? 0;
            input.IsLCFForward = viewModel.IsLCFForward ?? true;

            input.MTCPlus50Above = viewModel.MTCPlus50Above ?? 0;
            input.MTCPlus50Below = viewModel.MTCPlus50Below ?? 0;
            input.MTCMinus50Above = viewModel.MTCMinus50Above ?? 0;
            input.MTCMinus50Below = viewModel.MTCMinus50Below ?? 0;
            



            double displacementFromTable = 0;
            double tPCFromTable = 0;
            double lCFFromTable = 0;
            
            if (resultsDraughts?.MeanAdjustedDraught != null)
            {
                displacementFromTable = _surveyCalculationsService.CalculateLinearInterpolation(
                    input.DraughtAbove, 
                    input.DisplacementAbove, 
                    input.DraughtBelow, 
                    input.DisplacementBelow, 
                    resultsDraughts.MeanAdjustedDraught);

                tPCFromTable = _surveyCalculationsService.CalculateLinearInterpolation(
                    input.DraughtAbove,
                    input.TPCAbove,
                    input.DraughtBelow,
                    input.TPCBelow,
                    resultsDraughts.MeanAdjustedDraught);

                lCFFromTable = _surveyCalculationsService.CalculateLinearInterpolation(
                    input.DraughtAbove,
                    input.LCFAbove,
                    input.DraughtBelow,
                    input.LCFBelow,
                    resultsDraughts.MeanAdjustedDraught);
            }

            double mTCPlus50FromTable = 0;
            double mTCMinus50FromTable = 0;


            // Calculation of draughts +/- 50 cm, above and below 
            if (resultsDraughts?.MeanAdjustedDraught != null)
            {
                mTCPlus50FromTable = _surveyCalculationsService.CalculateLinearInterpolation(
                    input.DraughtAbove + 0.5,
                    input.MTCPlus50Above,
                    input.DraughtBelow + 0.5,
                    input.MTCPlus50Below,
                    resultsDraughts.MeanAdjustedDraught + 0.5
                    );

                mTCMinus50FromTable = _surveyCalculationsService.CalculateLinearInterpolation(
                    input.DraughtAbove - 0.5,
                    input.MTCMinus50Above,
                    input.DraughtBelow - 0.5,
                    input.MTCMinus50Below,
                    resultsDraughts.MeanAdjustedDraught - 0.5
                    );                
            }

            // Calculation of the 1st trim corrrection            

            double firstTrimCorrection = _surveyCalculationsService.CalculateFirstTrimCorrection(
                draughtSurveyBlock.DraughtsResults.TrimCorrected,
                draughtSurveyBlock.HydrostaticResults.LCFFromTable,
                draughtSurveyBlock.HydrostaticInput.IsLCFForward,
                draughtSurveyBlock.HydrostaticResults.TPCFromTable,
                draughtSurveyBlock.Inspection.VesselInput.LBP
                );


            // Calculcation of the 2nd trim correction
            double secondTrimCorrection = _surveyCalculationsService.CalculateSecondTrimCorrection(
                draughtSurveyBlock.DraughtsResults.TrimCorrected,
                draughtSurveyBlock.HydrostaticResults.MTCMinus50FromTable,
                draughtSurveyBlock.HydrostaticResults.MTCPlus50FromTable,
                draughtSurveyBlock.Inspection.VesselInput.LBP
                );


            // Calculation of Displacement corrected for trim corrections
            double displacementCorrectedForTrim = _surveyCalculationsService.CalculateDisplacementCorrectedForTrim(
                draughtSurveyBlock.HydrostaticResults.DisplacementFromTable,
                firstTrimCorrection,
                secondTrimCorrection
                );

            // Calculation of Displacement corrected for density
            double displacementCorrectedForDensity = _surveyCalculationsService.CalculateDisplacementCorrectedForDensity(
                displacementCorrectedForTrim,
                draughtSurveyBlock.DraughtsInput.SeaWaterDensity
                );

            // Calculation of NETTO displacement
            double nettoDisplacement = _surveyCalculationsService.CalculateNettoDisplacement(
                displacementCorrectedForDensity,
                draughtSurveyBlock.DeductiblesResults.TotalDeductibles
                );

            // Calculation of Cargo + Constant
            double cargoPlusConstant = _surveyCalculationsService.CalculateCargoPlusConstant(
                nettoDisplacement,
                draughtSurveyBlock.Inspection.VesselInput.LS
                );



            var results = draughtSurveyBlock.HydrostaticResults;

            //var results = await _context.HydrostaticResults
            //    .FirstOrDefaultAsync(r => r.DraughtSurveyBlockId == draughtSurveyBlock.Id);

            if (results == null)
            {
                results = new Models.HydrostaticResults
                {
                    DraughtSurveyBlockId = draughtSurveyBlock.Id,
                    DraughtSurveyBlock = draughtSurveyBlock
                };

                _context.HydrostaticResults.Add(results);
                draughtSurveyBlock.HydrostaticResults = results;
            }

            results.DisplacementFromTable = displacementFromTable;
            results.TPCFromTable = tPCFromTable;
            results.LCFFromTable = lCFFromTable;
            results.MTCPlus50FromTable = mTCPlus50FromTable;
            results.MTCMinus50FromTable = mTCMinus50FromTable;            
            results.FirstTrimCorrection = firstTrimCorrection;
            results.SecondTrimCorrection = secondTrimCorrection;
            results.DisplacementCorrectedForTrim = displacementCorrectedForTrim;
            results.DisplacementCorrectedForDensity = displacementCorrectedForDensity;
            results.NettoDisplacement = nettoDisplacement;
            results.CargoPlusConstant = cargoPlusConstant;


            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId });
        }
    }
}
