using DraughtSurveyWebApp.Data;
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





            //double? meanAdjustedDraught = results?.MeanAdjustedDraught;

            //double? DraughtAboveMTCPlus50 = null;
            //double? DraughtBelowMTC
            //if (meanAdjustedDraught.HasValue)
            //{
            //    DraughtAboveMTCPlus50 = _surveyCalculationsService.CalculateDraughtForMTCPlus50(meanAdjustedDraught.Value);
            //}


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


                DraughtAboveMTCPlus50 = draughtAboveMTCPlus50,
                DraughtAboveMTCMinus50 = draughtAboveMTCMinus50,
                DraughtBelowMTCPlus50 = draughtBelowMTCPlus50,
                DraughtBelowMTCMinus50 = draughtBelowMTCMinus50,

                MTCPlus50Above = inputs?.MTCPlus50Above,
                MTCPlus50Below = inputs?.MTCPlus50Below,
                MTCMinus50Above = inputs?.MTCMinus50Above,
                MTCMinus50Below = inputs?.MTCMinus50Below,

                MTCPlus50FromTable = resultsHydrostatic?.MTCPlus50FromTable,
                MTCMinus50FromTable = resultsHydrostatic?.MTCMinus50FromTable
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


            if (resultsDraughts?.MeanAdjustedDraught != null

                //viewModel.DraughtAboveMTCPlus50 != null ||
                //viewModel.DraughtBelowMTCPlus50 != null ||
                //viewModel.DraughtAboveMTCMinus50 != null ||
                //viewModel.DraughtBelowMTCMinus50 != null                

                )
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


                //mTCPlus50FromTable = _surveyCalculationsService.CalculateLinearInterpolation(
                //    viewModel.DraughtAboveMTCPlus50.Value,
                //    input.MTCPlus50Above,
                //    viewModel.DraughtBelowMTCPlus50.Value,
                //    input.MTCPlus50Below,
                //    resultsDraughts.MeanAdjustedDraught + 0.5
                //    );

                //mTCMinus50FromTable = _surveyCalculationsService.CalculateLinearInterpolation(
                //    viewModel.DraughtAboveMTCMinus50.Value,
                //    input.MTCMinus50Above,
                //    viewModel.DraughtBelowMTCMinus50.Value,
                //    input.MTCMinus50Below,
                //    resultsDraughts.MeanAdjustedDraught - 0.5
                //    );
            }




            var results = await _context.HydrostaticResults
                .FirstOrDefaultAsync(r => r.DraughtSurveyBlockId == draughtSurveyBlock.Id);

            if (results == null)
            {
                results = new Models.HydrostaticResults
                {
                    DraughtSurveyBlockId = draughtSurveyBlock.Id,
                    DraughtSurveyBlock = draughtSurveyBlock
                };

                _context.HydrostaticResults.Add(results);
            }

            results.DisplacementFromTable = displacementFromTable;
            results.TPCFromTable = tPCFromTable;
            results.LCFFromTable = lCFFromTable;
            results.MTCPlus50FromTable = mTCPlus50FromTable;
            results.MTCMinus50FromTable = mTCMinus50FromTable;


            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId });
        }




        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
