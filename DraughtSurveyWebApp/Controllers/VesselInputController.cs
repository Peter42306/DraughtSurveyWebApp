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
        private readonly ILogger<VesselInputController> _logger;   

        public VesselInputController(
            ApplicationDbContext context, 
            SurveyCalculationsService surveyCalculationsService,
            UserManager<ApplicationUser> userManager,
            ILogger<VesselInputController> logger)
        {
            _context = context;            
            _surveyCalculationsService = surveyCalculationsService;
            _userManager = userManager;
            _logger = logger;
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
            // Validate the view model
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Get the current user
            var user = await _userManager.GetUserAsync(User);

            // If the user is not logged in, redirect to login
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if the inspection exists and belongs to the user or if the user is an admin
            var inspection = await _context.Inspections.FirstOrDefaultAsync(i => i.Id == viewModel.InspectionId);
            if (inspection == null || (inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            // Check if a VesselInput already exists for this inspection
            var existing = await _context.VesselInputs                                
                .FirstOrDefaultAsync(v => v.InspectionId == viewModel.InspectionId);

            // If it exists, redirect to the Edit action
            if (existing != null) 
            {
                return RedirectToAction("Edit", new { InspectionId = viewModel.InspectionId});
            }

            // Check if the IMO is provided and if a previous VesselInput exists with the same IMO
            if (!string.IsNullOrEmpty(viewModel.IMO))
            {                
                var matchByIMO = await _context.VesselInputs                    
                    .Include(v => v.Inspection)                    
                    .Where(v => v.IMO == viewModel.IMO && v.Inspection.ApplicationUserId == user.Id)                    
                    .OrderByDescending(v => v.Id)                    
                    .FirstOrDefaultAsync();

                if (matchByIMO != null)
                {
                    _context.VesselInputs.Add(new VesselInput 
                    { 
                        Inspection = inspection,
                        InspectionId = viewModel.InspectionId,
                        IMO = matchByIMO.IMO, // Use the existing IMO
                        LBP = matchByIMO.LBP,
                        BM = matchByIMO.BM,
                        LS = matchByIMO.LS,
                        SDWT = matchByIMO.SDWT,
                        DeclaredConstant = matchByIMO.DeclaredConstant
                    }); 
                    
                    
                    await _context.SaveChangesAsync();

                    TempData["VesselAutoFilled"] = "Vessel information was automatically filled from your previous inspection of this vessel.";
                    return RedirectToAction("Details", "Inspections", new { id = viewModel.InspectionId });
                }
            }


            // Create a new VesselInput object, if no existing one is found
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

            

            bool changed = IsVesselInputChanged(vessel, viewModel);

            if (changed) 
            {
                vessel.IMO = viewModel.IMO;
                vessel.LBP = viewModel.LBP;
                vessel.BM = viewModel.BM;
                vessel.LS = viewModel.LS;
                vessel.SDWT = viewModel.SDWT;
                vessel.DeclaredConstant = viewModel.DeclaredConstant;
            }

            var draughtSurveyBlocks = await _context.DraughtSurveyBlocks
            .Where(b => b.InspectionId == inspectionId)
            .Include(b => b.DraughtsInput)
            .Include(b => b.DraughtsResults)
            .Include(b => b.HydrostaticInput)
            .Include(b => b.HydrostaticResults)
            .Include(b => b.DeductiblesInput)
            .Include(b => b.DeductiblesResults)
            .Include(b => b.Inspection).ThenInclude(b => b.CargoInput)
            .Include(b => b.Inspection).ThenInclude(b => b.CargoResult)
            .Include(b => b.Inspection).ThenInclude(i => i.VesselInput)
            .ToListAsync();

            foreach (var draughtSurveyBlock in draughtSurveyBlocks)
            {
                _surveyCalculationsService.RecalculateAll(draughtSurveyBlock);
            }



            // Recalculation of results if available             

            var inspection = vessel.Inspection;


            var initialBlock = draughtSurveyBlocks.FirstOrDefault(b => b.SurveyType == SurveyType.Initial);
            var finalBlock = draughtSurveyBlocks.FirstOrDefault(b => b.SurveyType == SurveyType.Final);
            var cargoResult = inspection?.CargoResult;
            

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

            // Calculate difference with SDWT
            var declaredWeight = inspection?.CargoInput?.DeclaredWeight;            

            if (cargoByDraughtSurvey.HasValue && declaredWeight.HasValue)
            {
                differenceWithBL_Mt = Math.Round(cargoByDraughtSurvey.Value - declaredWeight.Value, 3, MidpointRounding.AwayFromZero);
                differenceWithBL_Percents = Math.Round((differenceWithBL_Mt.Value / declaredWeight.Value) * 100, 3, MidpointRounding.AwayFromZero);
            }

            var sdwt = vessel.SDWT;
            
            if (differenceWithBL_Mt.HasValue && sdwt.HasValue)
            {
                DifferenceWithSDWT_Percents = Math.Round((differenceWithBL_Mt.Value / sdwt.Value) * 100, 3, MidpointRounding.AwayFromZero);
            }
            

            // Update CargoResult with calculated values


            if (cargoResult != null)
            {
                cargoResult.CargoByDraughtSurvey = cargoByDraughtSurvey;
                cargoResult.DifferenceWithBL_Mt = differenceWithBL_Mt;
                cargoResult.DifferenceWithBL_Percents = differenceWithBL_Percents;
                cargoResult.DifferenceWithSDWT_Percents = DifferenceWithSDWT_Percents;
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
