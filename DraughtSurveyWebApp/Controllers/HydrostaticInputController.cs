using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.Services;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            _logger.LogWarning("Before changed 1");
            if (changed)
            {
                _logger.LogWarning("Entered changed 2");

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


                if (viewModel.DraughtAbove.HasValue)
                {
                    _logger.LogWarning("Entered changed 3");

                    if (draughtSurveyBlock.Inspection.VesselInput != null)
                    {
                        var existingRowAbove = await _context.UserHydrostaticTableRows
                        .FirstOrDefaultAsync(r =>
                        r.ApplicationUserId == user.Id &&
                        r.VesselInputId == draughtSurveyBlock.Inspection.VesselInput.Id &&
                        Math.Abs(r.Draught - viewModel.DraughtAbove.Value) < 0.001);

                        if (existingRowAbove != null)
                        {
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
                                ApplicationUser = user,
                                ApplicationUserId = user.Id,

                                VesselInput = draughtSurveyBlock.Inspection.VesselInput,
                                VesselInputId = draughtSurveyBlock.Inspection.VesselInput.Id,
                                
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

                if (viewModel.DraughtBelow.HasValue)
                {


                    if (draughtSurveyBlock.Inspection.VesselInput != null)
                    {
                        var existingRowBelow = await _context.UserHydrostaticTableRows
                        .FirstOrDefaultAsync(r =>
                        r.ApplicationUserId == user.Id &&
                        r.VesselInputId == draughtSurveyBlock.Inspection.VesselInput.Id &&
                        Math.Abs(r.Draught - viewModel.DraughtBelow.Value) < 0.001);

                        if (existingRowBelow != null)
                        {
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

                                VesselInput = draughtSurveyBlock.Inspection.VesselInput,
                                VesselInputId = draughtSurveyBlock.Inspection.VesselInput.Id,

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
            _logger.LogWarning("Entered IsHydrostaticsInputChanged");
            _logger.LogWarning("dbValue: {1}, viewModelValue: {2}", dbValue.DisplacementBelow, viewModelValue.DisplacementBelow);
            _logger.LogWarning("dbValue: {1}, viewModelValue: {2}", dbValue.DisplacementAbove, viewModelValue.DisplacementAbove);
            

            return
                !Nullable.Equals(dbValue.DraughtAbove, viewModelValue.DraughtAbove) ||
                !Nullable.Equals(dbValue.DraughtBelow, viewModelValue.DraughtBelow) ||

                !Nullable.Equals(dbValue.DisplacementAbove, viewModelValue.DisplacementAbove) ||
                !Nullable.Equals(dbValue.DisplacementBelow, viewModelValue.DisplacementBelow) ||
                !Nullable.Equals(dbValue.TPCAbove, viewModelValue.TPCAbove) ||
                !Nullable.Equals(dbValue.TPCBelow, viewModelValue.TPCBelow) ||
                !Nullable.Equals(dbValue.LCFAbove, viewModelValue.LCFAbove) ||
                !Nullable.Equals(dbValue.LCFBelow, viewModelValue.LCFBelow) ||
                !Nullable.Equals(dbValue.IsLCFForward, viewModelValue.IsLCFForward) ||
    
                !Nullable.Equals(dbValue.MTCPlus50Above, viewModelValue.MTCPlus50Above) ||
                !Nullable.Equals(dbValue.MTCPlus50Below, viewModelValue.MTCPlus50Below) ||
                !Nullable.Equals(dbValue.MTCMinus50Above, viewModelValue.MTCMinus50Above) ||
                !Nullable.Equals(dbValue.MTCMinus50Below, viewModelValue.MTCMinus50Below);
        }
    }
}
