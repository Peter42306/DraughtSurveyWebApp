using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.Services;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DraughtSurveyWebApp.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class CargoInputController : Controller
    {
        private readonly ApplicationDbContext _context;        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SurveyCalculationsService _surveyCalculationsService;
        private readonly ILogger<CargoInputController> _logger;

        public CargoInputController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            SurveyCalculationsService surveyCalculationsService,
            ILogger<CargoInputController> logger)
        {
            _context = context;            
            _userManager = userManager;
            _surveyCalculationsService = surveyCalculationsService;
            _logger = logger;
        }

        // GET: CargoInput/Create?inspectionId=5
        public async Task<IActionResult> Create(int inspectionId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var inspection = await _context.Inspections
                .FirstOrDefaultAsync(i => i.Id == inspectionId);

            if (inspection == null || (inspection.ApplicationUserId !=user.Id && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            var existing = await _context.CargoInputs
                .FirstOrDefaultAsync(c => c.InspectionId == inspectionId);
            if (existing != null)
            {
                return RedirectToAction("Edit", new { inspectionId });
            }

            var viewModel = new CargoInputViewModel
            {
                InspectionId = inspectionId,
            };

            return View(viewModel);
        }

        // POST: CargoInput/Create        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CargoInputViewModel viewModel)
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

            var inspection = await _context.Inspections
                .FirstOrDefaultAsync(i => i.Id == viewModel.InspectionId);
            if (inspection == null || (inspection.ApplicationUserId !=user.Id && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            var existing = await _context.CargoInputs
                .FirstOrDefaultAsync(c => c.InspectionId == viewModel.InspectionId);
            if (existing != null)
            {
                return RedirectToAction("Edit", new { inspectionId = viewModel.InspectionId });
            }
            
            var cargo = new CargoInput
            {
                InspectionId = viewModel.InspectionId,
                Inspection = inspection,
                CargoName = viewModel.CargoName,
                DeclaredWeight = viewModel.DeclaredWeight,
                LoadingTerminal = viewModel.LoadingTerminal,
                BerthNumber = viewModel.BerthNumber,
                Shipper = viewModel.Shipper,
                Consignee = viewModel.Consignee,
                DischargingPort = viewModel.DischargingPort,                
            };

            _context.CargoInputs.Add(cargo);




            var draughtSurveyBlocks = await _context.DraughtSurveyBlocks
            .Where(b => b.InspectionId == inspection.Id)
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

            var sdwt = inspection?.VesselInput?.SDWT;
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

            return RedirectToAction("Details", "Inspections", new { id = viewModel.InspectionId});
        }

        // GET: CargoInput/Edit/5
        public async Task<IActionResult>Edit(int inspectionId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }



            var cargo = await _context.CargoInputs
                .Include(c => c.Inspection)
                .FirstOrDefaultAsync(c => c.InspectionId == inspectionId);

            if (cargo == null || (cargo.Inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            var viewModel = new CargoInputViewModel
            {
                Id = cargo.Id,
                InspectionId = cargo.InspectionId,
                CargoName = cargo.CargoName,
                DeclaredWeight = cargo.DeclaredWeight,
                LoadingTerminal = cargo.LoadingTerminal,
                BerthNumber = cargo.BerthNumber,
                Shipper = cargo.Shipper,
                Consignee = cargo.Consignee,
                DischargingPort = cargo.DischargingPort,
                OperationType = cargo.Inspection.OperationType
            };

            return View(viewModel);
        }

        // POST: CargoInput/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int inspectionId, CargoInputViewModel viewModel)
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

            var cargo = await _context.CargoInputs
                .Include(c => c.Inspection)
                .FirstOrDefaultAsync(c => c.InspectionId == inspectionId);
                        

            if (cargo == null || (cargo.Inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin")))
            {
                return Forbid();
            }

            bool changed = IsCargoInputChanged(cargo, viewModel);

            if (changed)
            {
                cargo.CargoName = viewModel.CargoName;
                cargo.DeclaredWeight = viewModel.DeclaredWeight;
                cargo.LoadingTerminal = viewModel.LoadingTerminal;
                cargo.BerthNumber = viewModel.BerthNumber;
                cargo.Shipper = viewModel.Shipper;
                cargo.Consignee = viewModel.Consignee;
                cargo.DischargingPort = viewModel.DischargingPort;
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
             
            var inspection = cargo.Inspection;

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

            var sdwt = inspection?.VesselInput?.SDWT;
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

            //return RedirectToAction("Details", "Inspections", new { id = cargo.InspectionId });
            return Redirect($"{Url.Action("Details", "Inspections", new { id = cargo.InspectionId })}#draught-cargo-input");
        }       

        private bool IsCargoInputChanged(CargoInput dbValue, CargoInputViewModel vmValue)
        {
            return
                dbValue.CargoName != vmValue.CargoName ||
                dbValue.DeclaredWeight != vmValue.DeclaredWeight ||
                dbValue.LoadingTerminal != vmValue.LoadingTerminal ||
                dbValue.BerthNumber != vmValue.BerthNumber ||
                dbValue.Shipper != vmValue.Shipper ||
                dbValue.Consignee != vmValue.Consignee ||
                dbValue.DischargingPort != vmValue.DischargingPort;
        }



               
    }
}
