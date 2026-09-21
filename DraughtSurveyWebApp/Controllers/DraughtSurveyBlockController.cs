using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DraughtSurveyWebApp.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class DraughtSurveyBlockController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public DraughtSurveyBlockController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (draughtSurveyBlock.Inspection == null)
            {
                return NotFound();
            }

            if (draughtSurveyBlock.Inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var viewModel = new EditSurveyTimesViewModel
            {
                DraughtSurveyBlockId = draughtSurveyBlock.Id,
                InspectionId = draughtSurveyBlock.InspectionId,
                SurveyType = draughtSurveyBlock.SurveyType,
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
            var draughtSurveyBlock = await _context.DraughtSurveyBlocks
                .Include(b => b.Inspection)
                .FirstOrDefaultAsync(b => b.Id == viewModel.DraughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }                        

            if (draughtSurveyBlock.Inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            viewModel.SurveyType = draughtSurveyBlock.SurveyType;
            viewModel.InspectionId = draughtSurveyBlock.InspectionId;

            if (!ModelState.IsValid)
            {
                ViewBag.InspectionId = viewModel.InspectionId;
                return View(viewModel);
            }

            var input = draughtSurveyBlock;

            bool changed = IsTimesInputChanged(input, viewModel);



            if (viewModel.SurveyTimeStart.HasValue && viewModel.SurveyTimeEnd.HasValue &&
                    viewModel.SurveyTimeEnd < viewModel.SurveyTimeStart)
            {
                ModelState.AddModelError("SurveyTimeEnd", "Survey end time cannot be ealier than start time.");
            }

            if (draughtSurveyBlock.SurveyType == SurveyType.Initial &&
                viewModel.SurveyTimeEnd.HasValue && 
                viewModel.CargoOperationsDateTime.HasValue &&
                viewModel.CargoOperationsDateTime < viewModel.SurveyTimeEnd)
            {
                ModelState.AddModelError(
                    nameof(viewModel.CargoOperationsDateTime), 
                    "Cargo operations cannot be started before the initial survey is completed.");
            }

            if (draughtSurveyBlock.SurveyType == SurveyType.Final &&
                viewModel.SurveyTimeStart.HasValue &&
                viewModel.CargoOperationsDateTime.HasValue &&
                viewModel.CargoOperationsDateTime > viewModel.SurveyTimeStart)
            {
                ModelState.AddModelError(
                    nameof(viewModel.CargoOperationsDateTime), 
                    "Cargo operations must be completed before the final survey starts.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.InspectionId = viewModel.InspectionId;
                return View(viewModel);
            }


            if (changed)
            {
                

                input.SurveyTimeStart = viewModel.SurveyTimeStart;
                input.SurveyTimeEnd = viewModel.SurveyTimeEnd;
                input.CargoOperationsDateTime = viewModel.CargoOperationsDateTime;
            }
            

            await _context.SaveChangesAsync();            

            string anchor = draughtSurveyBlock.SurveyType == SurveyType.Initial ? "initial-draught-times" : "final-draught-times";

            return Redirect($"{Url.Action("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId })}#{anchor}");
        }

        private bool IsTimesInputChanged(DraughtSurveyBlock dbValue, EditSurveyTimesViewModel viewModelValue)
        {
            return
                dbValue.SurveyTimeStart != viewModelValue.SurveyTimeStart ||
                dbValue.SurveyTimeEnd != viewModelValue.SurveyTimeEnd ||
                dbValue.CargoOperationsDateTime != viewModelValue.CargoOperationsDateTime;
        }


        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
