using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.Services;
using static DraughtSurveyWebApp.Utils.Utils;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DraughtSurveyWebApp.Utils;

namespace DraughtSurveyWebApp.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class HydrostaticInputController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SurveyCalculationsService _surveyCalculationsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HydrostaticInputController> _logger;

        public HydrostaticInputController(
            ApplicationDbContext context, 
            SurveyCalculationsService surveyCalculationsService,
            UserManager<ApplicationUser> userManager,
            ILogger<HydrostaticInputController> logger)
        {
            _context = context;
            _surveyCalculationsService = surveyCalculationsService;
            _userManager = userManager;
            _logger=logger;
        }

        // GET: HydrostaticInput/Edit?draughtSurveyBlockId=3
        public async Task<IActionResult> Edit(int draughtSurveyBlockId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var draughtSurveyBlock = await _context.DraughtSurveyBlocks
                .Include(h => h.HydrostaticInput)
                .Include(r => r.DraughtsInput)
                .Include(r => r.DraughtsResults)
                .Include(r => r.HydrostaticResults)
                .Include(i => i.Inspection)
                    .ThenInclude(i => i.VesselInput)
                .FirstOrDefaultAsync(b => b.Id == draughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }

            if (draughtSurveyBlock.Inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Auto-fill hydrostatic input if available
            await AutoFillHydrostaticInputIfAvailable(draughtSurveyBlock, user);

            // Recalculate all survey calculations
            _surveyCalculationsService.RecalculateAll(draughtSurveyBlock);



            var inputs = draughtSurveyBlock.HydrostaticInput;
            var resultsDraughts = draughtSurveyBlock.DraughtsResults;
            var resultsHydrostatic = draughtSurveyBlock.HydrostaticResults;


            double? draughtAboveMTCPlus50 = null;
            double? draughtBelowMTCPlus50 = null;
            double? draughtAboveMTCMinus50 = null;
            double? draughtBelowMTCMinus50 = null;

            double? draughtAbove = inputs?.DraughtAbove;
            double? draughtBelow = inputs?.DraughtBelow;


            if (draughtAbove.HasValue)
            {
                draughtAboveMTCPlus50 = _surveyCalculationsService.CalculateDraughtForMTCPlus50(draughtAbove.Value);
                draughtAboveMTCMinus50 = _surveyCalculationsService.CalculateDraughtForMTCMinus50(draughtAbove.Value);                
            }

            if (draughtBelow.HasValue)
            {                
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

            

            
            viewModel.Inspection = draughtSurveyBlock.Inspection;
            viewModel.DraughtsInput = draughtSurveyBlock.DraughtsInput;
            viewModel.VesselInput = draughtSurveyBlock.Inspection.VesselInput;
            viewModel.DeductiblesResults = draughtSurveyBlock.DeductiblesResults;

            return View(viewModel);
        }


        // POST: 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HydrostaticInputViewModel viewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }


            var draughtSurveyBlock = await _context.DraughtSurveyBlocks
                .Include(b => b.DraughtsInput)
                .Include(r => r.DraughtsResults)
                .Include(h => h.HydrostaticInput)
                .Include(r => r.HydrostaticResults)
                .Include(b => b.DeductiblesInput)
                .Include(r => r.DeductiblesResults)
                .Include(i => i.Inspection)
                    .ThenInclude(i => i.VesselInput)                
                .FirstOrDefaultAsync(b => b.Id == viewModel.DraughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }

            if (draughtSurveyBlock.Inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
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

            if (draughtSurveyBlock.HydrostaticResults == null)
            {
                draughtSurveyBlock.HydrostaticResults = new Models.HydrostaticResults
                {
                    DraughtSurveyBlockId = draughtSurveyBlock.Id,
                    DraughtSurveyBlock = draughtSurveyBlock
                };
                
                _context.HydrostaticResults.Add(draughtSurveyBlock.HydrostaticResults);
            }


            var input = draughtSurveyBlock.HydrostaticInput;
            //var results = draughtSurveyBlock.HydrostaticResults;             
            //var resultsDraughts = draughtSurveyBlock.DraughtsResults;            

            bool changed = IsHydrostaticsInputChanged(input, viewModel);

            
            if (changed)
            {
                

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


                if (viewModel.DraughtAbove.HasValue &&
                    viewModel.DisplacementAbove.HasValue &&
                    viewModel.TPCAbove.HasValue &&
                    viewModel.LCFAbove.HasValue && 
                    viewModel.IsLCFForward.HasValue &&
                    viewModel.MTCPlus50Above.HasValue &&
                    viewModel.MTCMinus50Above.HasValue)
                {
                    

                    if (draughtSurveyBlock.Inspection.VesselInput != null)
                    {
                        var existingRowAbove = await _context.UserHydrostaticTableRows
                            .FirstOrDefaultAsync(r =>
                            r.ApplicationUserId == user.Id &&                        
                            r.IMO == draughtSurveyBlock.Inspection.VesselInput.IMO &&
                            Math.Abs(r.Draught - viewModel.DraughtAbove.Value) < 0.001);

                        if (existingRowAbove != null)
                        {
                            existingRowAbove.Draught = viewModel.DraughtAbove.Value;
                            existingRowAbove.Displacement = viewModel.DisplacementAbove;
                            existingRowAbove.TPC = viewModel.TPCAbove;
                            existingRowAbove.LCF = viewModel.LCFAbove;
                            existingRowAbove.IsLcfForward = viewModel.IsLCFForward;
                            existingRowAbove.MTCPlus50 = viewModel.MTCPlus50Above;
                            existingRowAbove.MTCMinus50 = viewModel.MTCMinus50Above;
                        }
                        else
                        {
                            var newRow = new UserHydrostaticTableRow
                            {
                                ApplicationUserId = user.Id,
                                ApplicationUser = user,                                
                                
                                IMO = draughtSurveyBlock.Inspection.VesselInput.IMO,
                                VesselName = draughtSurveyBlock.Inspection.VesselName,

                                Draught = viewModel.DraughtAbove.Value,
                                Displacement = viewModel.DisplacementAbove,
                                TPC = viewModel.TPCAbove,
                                LCF = viewModel.LCFAbove,
                                IsLcfForward = viewModel.IsLCFForward,
                                MTCPlus50 = viewModel.MTCPlus50Above,
                                MTCMinus50 = viewModel.MTCMinus50Above                                

                            };                            

                            _context.UserHydrostaticTableRows.Add(newRow);

                        }

                    }

                    
                    
                }

                if (viewModel.DraughtBelow.HasValue &&
                    viewModel.DisplacementBelow.HasValue &&
                    viewModel.TPCBelow.HasValue &&
                    viewModel.LCFBelow.HasValue &&
                    viewModel.IsLCFForward.HasValue &&
                    viewModel.MTCPlus50Below.HasValue &&
                    viewModel.MTCMinus50Below.HasValue)
                {


                    if (draughtSurveyBlock.Inspection.VesselInput != null)
                    {
                        var existingRowBelow = await _context.UserHydrostaticTableRows
                            .FirstOrDefaultAsync(r =>
                            r.ApplicationUserId == user.Id &&                        
                            r.IMO == draughtSurveyBlock.Inspection.VesselInput.IMO &&
                            Math.Abs(r.Draught - viewModel.DraughtBelow.Value) < 0.001);

                        if (existingRowBelow != null)
                        {
                            existingRowBelow.Draught = viewModel.DraughtBelow.Value;
                            existingRowBelow.Displacement = viewModel.DisplacementBelow;
                            existingRowBelow.TPC = viewModel.TPCBelow;
                            existingRowBelow.LCF = viewModel.LCFBelow;
                            existingRowBelow.IsLcfForward = viewModel.IsLCFForward;
                            existingRowBelow.MTCPlus50 = viewModel.MTCPlus50Below;
                            existingRowBelow.MTCMinus50 = viewModel.MTCMinus50Below;
                        }
                        else
                        {
                            var newRow = new UserHydrostaticTableRow
                            {
                                ApplicationUser = user,
                                ApplicationUserId = user.Id,                                

                                IMO = draughtSurveyBlock.Inspection.VesselInput.IMO,
                                VesselName = draughtSurveyBlock.Inspection.VesselName,

                                Draught = viewModel.DraughtBelow.Value,
                                Displacement = viewModel.DisplacementBelow,
                                TPC = viewModel.TPCBelow,
                                LCF = viewModel.LCFBelow,
                                IsLcfForward = viewModel.IsLCFForward,
                                MTCPlus50 = viewModel.MTCPlus50Below,
                                MTCMinus50 = viewModel.MTCMinus50Below

                            };

                            _context.UserHydrostaticTableRows.Add(newRow);
                        }


                    }



                }



                //if (viewModel.DraughtAbove.HasValue)
                //{
                //    //var existingRow = await _context.UserHydrostaticTableRows
                //    //    .FirstOrDefaultAsync(r =>
                //    //    r.ApplicationUserId == user.Id &&
                //        //r.VesselInputId == draughtSurveyBlock.Inspection.VesselInputId &&

                //        );


                //    r => r.DraughtSurveyBlockId == draughtSurveyBlock.Id && 
                //        r.Draught == viewModel.DraughtAbove.Value);
                //}
            }

            


            _surveyCalculationsService.RecalculateAll(draughtSurveyBlock);

            await _context.SaveChangesAsync();

            //return RedirectToAction("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId });
            return Redirect($"{Url.Action("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId })}#initial-draught-hydrostatics");
        }


        private bool IsHydrostaticsInputChanged(HydrostaticInput dbValue, HydrostaticInputViewModel viewModelValue)
        {           

            return                

                !AreEqual(dbValue.DraughtAbove, viewModelValue.DraughtAbove) ||
                !AreEqual(dbValue.DraughtBelow, viewModelValue.DraughtBelow) ||
                !AreEqual(dbValue.DisplacementAbove, viewModelValue.DisplacementAbove) ||
                !AreEqual(dbValue.DisplacementBelow, viewModelValue.DisplacementBelow) ||
                !AreEqual(dbValue.TPCAbove, viewModelValue.TPCAbove) ||
                !AreEqual(dbValue.TPCBelow, viewModelValue.TPCBelow) ||
                !AreEqual(dbValue.LCFAbove, viewModelValue.LCFAbove) ||
                !AreEqual(dbValue.LCFBelow, viewModelValue.LCFBelow) ||
                !AreEqual(dbValue.IsLCFForward, viewModelValue.IsLCFForward) ||
                !AreEqual(dbValue.MTCPlus50Above, viewModelValue.MTCPlus50Above) ||
                !AreEqual(dbValue.MTCPlus50Below, viewModelValue.MTCPlus50Below) ||
                !AreEqual(dbValue.MTCMinus50Above, viewModelValue.MTCMinus50Above) ||
                !AreEqual(dbValue.MTCMinus50Below, viewModelValue.MTCMinus50Below);
        }

        private async Task AutoFillHydrostaticInputIfAvailable(DraughtSurveyBlock block, ApplicationUser user)
        {
            var draughtCalculated = block.DraughtsResults?.MeanAdjustedDraughtAfterKeelCorrection;
            var imo = block.Inspection.VesselInput?.IMO;

            if (!draughtCalculated.HasValue || string.IsNullOrWhiteSpace(imo))
            {
                return;
            }

            var hydrostaticRows = await _context.UserHydrostaticTableRows
                .Where(r => r.ApplicationUserId == user.Id && r.IMO == imo)
                .OrderBy(r => r.Draught)
                .ToListAsync();

            if (!hydrostaticRows.Any())
            {
                return;
            }

            UserHydrostaticTableRow? lowerRow = null;
            UserHydrostaticTableRow? upperRow = null;

            foreach (var row in hydrostaticRows)
            {
                double tolerance = 0.001;

                if (Math.Abs(row.Draught - draughtCalculated.Value) < tolerance)
                {
                    lowerRow = row;
                    upperRow = row;
                    break;
                }                
                
                if (row.Draught <= draughtCalculated.Value)
                {
                    upperRow = row;
                }

                if (row.Draught >= draughtCalculated.Value)
                {
                    lowerRow = row;
                    break;
                }
            }

            if (lowerRow == null || upperRow == null)
            {
                return;
            }

            block.HydrostaticInput ??= new HydrostaticInput
            {
                DraughtSurveyBlockId = block.Id,
                DraughtSurveyBlock = block
            };

            block.HydrostaticInput.DraughtBelow = lowerRow.Draught;
            block.HydrostaticInput.DraughtAbove = upperRow.Draught;

            block.HydrostaticInput.DisplacementBelow = lowerRow.Displacement;
            block.HydrostaticInput.DisplacementAbove = upperRow.Displacement;

            block.HydrostaticInput.TPCBelow = lowerRow.TPC;
            block.HydrostaticInput.TPCAbove = upperRow.TPC;

            block.HydrostaticInput.LCFBelow = lowerRow.LCF;
            block.HydrostaticInput.LCFAbove = upperRow.LCF;

            block.HydrostaticInput.IsLCFForward = lowerRow.IsLcfForward ?? upperRow.IsLcfForward;

            block.HydrostaticInput.MTCPlus50Below = lowerRow.MTCPlus50;
            block.HydrostaticInput.MTCPlus50Above = upperRow.MTCPlus50;

            block.HydrostaticInput.MTCMinus50Below = lowerRow.MTCMinus50;
            block.HydrostaticInput.MTCMinus50Above = upperRow.MTCMinus50;

        }
        
    }
}
