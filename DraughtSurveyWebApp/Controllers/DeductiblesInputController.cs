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
    public class DeductiblesInputController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SurveyCalculationsService _surveyCalculationsService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeductiblesInputController(
            ApplicationDbContext context, 
            SurveyCalculationsService surveyCalculationsService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _surveyCalculationsService = surveyCalculationsService;
            _userManager = userManager;
        }

        // GET: DeductiblesInput/Edit?draughtSurveyBlockId=1
        public async Task<IActionResult> Edit(int draughtSurveyBlockId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var draughtSurveyBlock = await _context.DraughtSurveyBlocks
                .Include(d => d.DeductiblesInput)
                .Include(d => d.DeductiblesResults)
                .Include(i => i.Inspection)
                .FirstOrDefaultAsync(b => b.Id == draughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }

            if (draughtSurveyBlock.Inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var inputs = draughtSurveyBlock.DeductiblesInput;
            var results = draughtSurveyBlock.DeductiblesResults;

            var viewModel = new DeductiblesInputViewModel
            {
                InspectionId = draughtSurveyBlock.InspectionId,
                DraughtSurveyBlockId = draughtSurveyBlock.Id,

                Ballast = inputs?.Ballast,
                FreshWater = inputs?.FreshWater,
                FuelOil = inputs?.FuelOil,
                DieselOil = inputs?.DieselOil,
                LubOil = inputs?.LubOil,
                Others = inputs?.Others,

                TotalDeductibles = results?.TotalDeductibles
            };

            return View(viewModel);
        }

        // POST: 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DeductiblesInputViewModel viewModel)
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
                .Include(b => b.DraughtsResults)
                .Include(b => b.HydrostaticInput)
                .Include(b => b.HydrostaticResults)
                .Include(b => b.DeductiblesInput)
                .Include(b => b.DeductiblesResults)                
                .Include(b => b.Inspection)
                    .ThenInclude(i => i.VesselInput)
                .Include(b => b.Inspection)
                    .ThenInclude(i => i.CargoResult)
                .Include(b => b.Inspection)
                    .ThenInclude(i => i.CargoInput)
                .Include(b => b.Inspection)
                    .ThenInclude(i => i.DraughtSurveyBlocks)
                .FirstOrDefaultAsync(b => b.Id == viewModel.DraughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }

            if (draughtSurveyBlock.Inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (draughtSurveyBlock.DeductiblesInput == null)
            {
                draughtSurveyBlock.DeductiblesInput = new DeductiblesInput
                {
                    DraughtSurveyBlockId = viewModel.DraughtSurveyBlockId,
                    DraughtSurveyBlock = draughtSurveyBlock
                };

                _context.DeductiblesInputs.Add(draughtSurveyBlock.DeductiblesInput);
            }

            if (draughtSurveyBlock.DeductiblesResults == null)
            {
                draughtSurveyBlock.DeductiblesResults = new DeductiblesResults
                {
                    DraughtSurveyBlockId = draughtSurveyBlock.Id,
                    DraughtSurveyBlock = draughtSurveyBlock
                };

                _context.DeductiblesResults.Add(draughtSurveyBlock.DeductiblesResults);
            }


            var inputs = draughtSurveyBlock.DeductiblesInput;

            bool changed = IsDeductiblesInputChanged(inputs, viewModel);

            if (changed)
            {
                inputs.Ballast = viewModel.Ballast;
                inputs.FreshWater = viewModel.FreshWater;
                inputs.FuelOil = viewModel.FuelOil;
                inputs.DieselOil = viewModel.DieselOil;
                inputs.LubOil = viewModel.LubOil;
                inputs.Others = viewModel.Others;
            }

            

            _surveyCalculationsService.RecalculateAll(draughtSurveyBlock);



            // Recalculation of results if available 

            var initialBlock = await _context.DraughtSurveyBlocks
                .Include(b => b.HydrostaticResults)
                .FirstOrDefaultAsync(b =>
                b.InspectionId == draughtSurveyBlock.InspectionId &&
                b.SurveyType == SurveyType.Initial);

            var finalBlock = await _context.DraughtSurveyBlocks
                .Include(b => b.HydrostaticResults)
                .FirstOrDefaultAsync(b =>
                b.InspectionId == draughtSurveyBlock.InspectionId &&
                b.SurveyType == SurveyType.Final);

            var initialNetto = initialBlock?.HydrostaticResults?.NettoDisplacement;
            var finalNetto = finalBlock?.HydrostaticResults?.NettoDisplacement;


            double? cargoByDraughtSurvey = null;
            double? differenceWithBL_Mt = null;
            double? differenceWithBL_Percents = null;
            double? DifferenceWithSDWT_Percents = null;

            // Calculate cargo by draught survey
            if (initialNetto.HasValue && finalNetto.HasValue)
            {
                cargoByDraughtSurvey = Math.Abs(Math.Round(finalNetto.Value - initialNetto.Value, 3, MidpointRounding.AwayFromZero));                
            }                       

            if (draughtSurveyBlock.Inspection.CargoResult == null)
            {
                draughtSurveyBlock.Inspection.CargoResult = new CargoResult
                {
                    InspectionId = draughtSurveyBlock.InspectionId,
                    Inspection = draughtSurveyBlock.Inspection
                };
            }


            // Calculate difference with Bill of Lading
            var declaredWeight = draughtSurveyBlock.Inspection.CargoInput?.DeclaredWeight;

            if (cargoByDraughtSurvey.HasValue && declaredWeight.HasValue)
            {
                differenceWithBL_Mt = Math.Round(cargoByDraughtSurvey.Value - declaredWeight.Value, 3, MidpointRounding.AwayFromZero);
                differenceWithBL_Percents = Math.Round((differenceWithBL_Mt.Value / declaredWeight.Value) * 100, 3, MidpointRounding.AwayFromZero);
            }

            // Calculate difference with SDWT
            var sdwt = draughtSurveyBlock.Inspection.VesselInput?.SDWT;
            
            if (differenceWithBL_Mt.HasValue && sdwt.HasValue)
            {
                DifferenceWithSDWT_Percents = Math.Round((differenceWithBL_Mt.Value / sdwt.Value) * 100, 3, MidpointRounding.AwayFromZero);
            }


            // Update CargoResult with calculated values
            draughtSurveyBlock.Inspection.CargoResult.CargoByDraughtSurvey = cargoByDraughtSurvey;
            draughtSurveyBlock.Inspection.CargoResult.DifferenceWithBL_Mt = differenceWithBL_Mt;
            draughtSurveyBlock.Inspection.CargoResult.DifferenceWithBL_Percents = differenceWithBL_Percents;
            draughtSurveyBlock.Inspection.CargoResult.DifferenceWithSDWT_Percents = DifferenceWithSDWT_Percents;


            await _context.SaveChangesAsync();

            string anchor = draughtSurveyBlock.SurveyType == SurveyType.Initial 
                ? "initial-draught-deductibles" 
                : "final-draught-deductibles";

            return Redirect($"{Url.Action("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId })}#{anchor}");
        }

        private bool IsDeductiblesInputChanged(DeductiblesInput dbValue, DeductiblesInputViewModel viewModelValue)
        {
            return
                dbValue.Ballast != viewModelValue.Ballast ||
                dbValue.FreshWater != viewModelValue.FreshWater ||
                dbValue.FuelOil != viewModelValue.FuelOil ||
                dbValue.DieselOil != viewModelValue.DieselOil ||
                dbValue.LubOil != viewModelValue.LubOil ||
                dbValue.Others != viewModelValue.Others;
        }

    }
}
