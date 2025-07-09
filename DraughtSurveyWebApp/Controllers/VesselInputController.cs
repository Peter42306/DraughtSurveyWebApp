using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.Services;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DraughtSurveyWebApp.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class VesselInputController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SurveyCalculationsService _surveyCalculationsService;
        private readonly UserManager<ApplicationUser> _userManager;

        public VesselInputController(
            ApplicationDbContext context, 
            SurveyCalculationsService surveyCalculationsService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;            
            _surveyCalculationsService = surveyCalculationsService;
            _userManager = userManager;
        }

        // GET: VesselInput/Create?inspectionId=5
        public async Task<IActionResult> Create(int inspectionId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) 
            {
                return RedirectToAction("Login", "Account");
            }

            var inspection = await _context.Inspections.FirstOrDefaultAsync(i => i.Id == inspectionId);
            if (inspection == null || (inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            var existing = await _context.VesselInputs.FirstOrDefaultAsync(v => v.InspectionId == inspectionId);
            if (existing != null)
            {
                return RedirectToAction("Edit", new { inspectionId });
            }

            var viewModel = new VesselInputViewModel
            {
                InspectionId = inspectionId 
            };
                        
            return View(viewModel);
        }

        // POST: VesselInput/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VesselInputViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var inspection = await _context.Inspections.FirstOrDefaultAsync(i => i.Id == viewModel.InspectionId);
            if (inspection == null || (inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin")))
            {
                return Forbid();
            }            

            var existing = await _context.VesselInputs                                
                .FirstOrDefaultAsync(v => v.InspectionId == viewModel.InspectionId);

            if (existing != null) 
            {
                return RedirectToAction("Edit", new { InspectionId = viewModel.InspectionId});
            }

            var vessel = new VesselInput
            {
                InspectionId = viewModel.InspectionId,
                Inspection = inspection,
                IMO = viewModel.IMO,
                LBP = viewModel.LBP,
                BM = viewModel.BM,
                LS = viewModel.LS,
                SDWT = viewModel.SDWT,
                DeclaredConstant = viewModel.DeclaredConstant
            };

            _context.VesselInputs.Add(vessel);
            await _context.SaveChangesAsync();

            //return Content("REACHED GET Create");
            return RedirectToAction("Details", "Inspections", new { id = viewModel.InspectionId });
        }

        // GET: VesselInput/Edit/5
        public async Task<IActionResult> Edit(int inspectionId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var vessel = await _context.VesselInputs
                .Include(v => v.Inspection)
                .FirstOrDefaultAsync(v => v.InspectionId == inspectionId);                

            if(vessel == null || (vessel.Inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            var viewModel = new VesselInputViewModel
            {
                Id = vessel.Id,
                InspectionId = vessel.InspectionId,
                IMO = vessel.IMO,
                LBP = vessel.LBP,
                BM = vessel.BM,
                LS = vessel.LS,
                SDWT = vessel.SDWT,
                DeclaredConstant = vessel.DeclaredConstant
            };

            return View(viewModel);
        }

        // POST: VesselInput/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int inspectionId, VesselInputViewModel viewModel)
        {
            if (inspectionId != viewModel.InspectionId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var vessel = await _context.VesselInputs
                .Include(v => v.Inspection)
                .FirstOrDefaultAsync(v => v.InspectionId == inspectionId);                

            if (vessel == null || (vessel.Inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            

            bool isVesselInputChanged = IsVesselInputChanged(vessel, viewModel);

            if (isVesselInputChanged) 
            {
                vessel.IMO = viewModel.IMO;
                vessel.LBP = viewModel.LBP;
                vessel.BM = viewModel.BM;
                vessel.LS = viewModel.LS;
                vessel.SDWT = viewModel.SDWT;
                vessel.DeclaredConstant = viewModel.DeclaredConstant;                

                var draughtSurveyBlocks = await _context.DraughtSurveyBlocks
                .Where(b => b.InspectionId == inspectionId)
                .Include(b => b.DraughtsInput)
                .Include(b => b.DraughtsResults)
                .Include(b => b.HydrostaticInput)
                .Include(b => b.HydrostaticResults)
                .Include(b => b.DeductiblesInput)
                .Include(b => b.DeductiblesResults)
                .Include(b => b.Inspection)
                    .ThenInclude(i => i.VesselInput)
                .ToListAsync();

                foreach (var draughtSurveyBlock in draughtSurveyBlocks)
                {
                    _surveyCalculationsService.RecalculateAll(draughtSurveyBlock);
                }
            }
                        

            await _context.SaveChangesAsync();

            //return RedirectToAction("Details", "Inspections", new { id = vessel.InspectionId});
            return Redirect($"{Url.Action("Details", "Inspections", new { id = vessel.InspectionId })}#draught-vessel-input");
        }


        private bool IsVesselInputChanged(VesselInput dbValue, VesselInputViewModel viewModelValue)
        {
            return                

                dbValue.LBP != viewModelValue.LBP ||
                dbValue.BM != viewModelValue.BM ||
                dbValue.LS != viewModelValue.LS ||
                dbValue.SDWT != viewModelValue.SDWT ||
                dbValue.DeclaredConstant != viewModelValue.DeclaredConstant;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
