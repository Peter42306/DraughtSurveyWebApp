using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.Services;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DraughtSurveyWebApp.Controllers
{
    public class DeductiblesInputController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SurveyCalculationsService _surveyCalculationsService;

        public DeductiblesInputController(ApplicationDbContext context, SurveyCalculationsService surveyCalculationsService)
        {
            _context = context;
            _surveyCalculationsService = surveyCalculationsService;
        }

        // GET: DeductiblesInput/Edit?draughtSurveyBlockId=1
        public async Task<IActionResult> Edit(int draughtSurveyBlockId)
        {
            var draughtSurveyBlock = await _context.DraughtSurveyBlocks
                .Include(d => d.DeductiblesInput)
                .Include(d => d.DeductiblesResults)
                .FirstOrDefaultAsync(b => b.Id == draughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
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
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var draughtSurveyBlock = await _context.DraughtSurveyBlocks
                .Include(d => d.DeductiblesInput)
                .Include(d => d.DeductiblesResults)
                .FirstOrDefaultAsync(b => b.Id == viewModel.DraughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }

            if(draughtSurveyBlock.DeductiblesInput == null)
            {
                draughtSurveyBlock.DeductiblesInput = new Models.DeductiblesInput
                {
                    DraughtSurveyBlockId = viewModel.DraughtSurveyBlockId,
                    DraughtSurveyBlock = draughtSurveyBlock
                };

                _context.DeductiblesInputs.Add(draughtSurveyBlock.DeductiblesInput);
            }
            
            var inputs = draughtSurveyBlock.DeductiblesInput;

            inputs.Ballast = viewModel.Ballast;
            inputs.FreshWater = viewModel.FreshWater;
            inputs.FuelOil = viewModel.FuelOil;
            inputs.DieselOil = viewModel.DieselOil;
            inputs.LubOil = viewModel.LubOil;
            inputs.Others = viewModel.Others;

            double? totalDeductible = null;

            if (inputs.Ballast.HasValue ||
                inputs.FreshWater.HasValue ||
                inputs.FuelOil.HasValue ||
                inputs.DieselOil.HasValue ||
                inputs.LubOil.HasValue ||
                inputs.Others.HasValue)
            {
                totalDeductible =
                (inputs.Ballast ?? 0) +
                (inputs.FreshWater ?? 0) +
                (inputs.FuelOil ?? 0) +
                (inputs.DieselOil ?? 0) +
                (inputs.LubOil ?? 0) +
                (inputs.Others ?? 0);
            }
            

            var results = draughtSurveyBlock.DeductiblesResults;

            if (results == null)
            {
                results = new Models.DeductiblesResults
                {
                    DraughtSurveyBlockId = draughtSurveyBlock.Id,
                    DraughtSurveyBlock = draughtSurveyBlock
                };

                _context.DeductiblesResults.Add(results);
            }

            results.TotalDeductibles = totalDeductible;            


            await _context.SaveChangesAsync();

            //return RedirectToAction("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId });
            return Redirect($"{Url.Action("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId })}#initial-draught-deductibles");
        }


        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
