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
