using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DraughtSurveyWebApp.Controllers
{
    public class DraughtSurveyBlockController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DraughtSurveyBlockController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DraughtSurveyBlock/EditTimes/5
        public async Task<IActionResult> EditTimes(int draughtSurveyBlockId)
        {
            var draughtSurveyBlock = await _context.DraughtSurveyBlocks                
                .Include(b => b.Inspection)
                .FirstOrDefaultAsync(b => b.Id == draughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }

            var viewModel = new EditSurveyTimesViewModel
            {
                DraughtSurveyBlockId = draughtSurveyBlock.Id,
                SurveyTimeStart = draughtSurveyBlock.SurveyTimeStart,
                SurveyTimeEnd = draughtSurveyBlock.SurveyTimeEnd,
                CargoOperationsDateTime = draughtSurveyBlock.CargoOperationsDateTime
            };

            ViewBag.InspectionId = draughtSurveyBlock.InspectionId;

            return View(viewModel);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTimes(EditSurveyTimesViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.InspectionId = viewModel.InspectionId;
                return View(viewModel);
            }

            var draughtSurveyBlock = await _context.DraughtSurveyBlocks
                .Include(b => b.Inspection)
                .FirstOrDefaultAsync(b => b.Id == viewModel.DraughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }

            draughtSurveyBlock.SurveyTimeStart = viewModel.SurveyTimeStart;
            draughtSurveyBlock.SurveyTimeEnd = viewModel.SurveyTimeEnd;
            draughtSurveyBlock.CargoOperationsDateTime = viewModel.CargoOperationsDateTime;

            await _context.SaveChangesAsync();

            //return RedirectToAction("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId });
            return Redirect($"{Url.Action("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId })}#initial-draught-times");
        }


        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
