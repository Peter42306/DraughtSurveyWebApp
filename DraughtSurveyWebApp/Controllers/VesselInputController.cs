using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.Services;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DraughtSurveyWebApp.Controllers
{
    public class VesselInputController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SurveyCalculationsService _surveyCalculationsService;

        public VesselInputController(ApplicationDbContext context, SurveyCalculationsService surveyCalculationsService)
        {
            _context = context;            
            _surveyCalculationsService = surveyCalculationsService;
        }

        // GET: VesselInput/Create?inspectionId=5
        public async Task<IActionResult> Create(int inspectionId)
        {
            var existing = await _context.VesselInputs
                .FirstOrDefaultAsync(v => v.InspectionId == inspectionId);

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

            var vessel = new VesselInput
            {
                InspectionId = viewModel.InspectionId,
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
            var vessel = await _context.VesselInputs
                .FirstOrDefaultAsync(v => v.InspectionId == inspectionId);                

            if(vessel == null)
            {
                return NotFound();
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

            var vessel = await _context.VesselInputs
                .FirstOrDefaultAsync(v => v.InspectionId == inspectionId);                

            if (vessel == null)
            {
                return NotFound();
            }

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
                if (draughtSurveyBlock != null)
                {
                    _surveyCalculationsService.RecalculateAll(draughtSurveyBlock);
                }                    
            }
            

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Inspections", new { id = vessel.InspectionId});
        }



        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
