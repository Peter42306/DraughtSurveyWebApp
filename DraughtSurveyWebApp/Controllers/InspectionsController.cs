using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using DraughtSurveyWebApp.Services;

namespace DraughtSurveyWebApp.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class InspectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SurveyCalculationsService _surveyCalculationsService;
        private readonly UserManager<ApplicationUser> _userManager;

        public InspectionsController(
            ApplicationDbContext context,
            SurveyCalculationsService surveyCalculationsService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _surveyCalculationsService = surveyCalculationsService;
            _userManager = userManager;
        }

        // GET: Inspections
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var inspections = User.IsInRole("Admin")
                ? await _context.Inspections
                    .OrderByDescending(i => i.Id)
                    .ToListAsync()
                : await _context.Inspections
                    .Where(i => i.ApplicationUserId == user.Id)
                    .OrderByDescending(i => i.Id)
                    .ToListAsync();                

            return View(inspections);
        }

        // GET: Inspections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var inspection = await _context.Inspections
                .Include(i => i.VesselInput)
                .Include(i => i.CargoInput)
                .Include(i => i.DraughtSurveyBlocks).ThenInclude(b => b.DraughtsInput)                
                .Include(i => i.DraughtSurveyBlocks).ThenInclude(b => b.DraughtsResults)
                .Include(i => i.DraughtSurveyBlocks).ThenInclude(b => b.HydrostaticInput)
                .Include(i => i.DraughtSurveyBlocks).ThenInclude(b => b.HydrostaticResults)
                .Include(i => i.DraughtSurveyBlocks).ThenInclude(b => b.DeductiblesInput)
                .Include(i => i.DraughtSurveyBlocks).ThenInclude(b => b.DeductiblesResults)
                .FirstOrDefaultAsync(m => m.Id == id);
                        
            if (inspection == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin") && inspection.ApplicationUserId != user.Id) 
            {
                return NotFound();
            }

            foreach (var block in inspection.DraughtSurveyBlocks)
            {
                _surveyCalculationsService.RecalculateAll(block);
            }
            
            
            return View(inspection);
        }

        // GET: Inspections/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inspections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InspectionCreateViewModel viewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }            


            var inspection = new Inspection
            {
                ApplicationUserId = user.Id,

                VesselName = viewModel.VesselName,
                Port = viewModel.Port,
                CompanyReference = viewModel.CompanyReference,
                OperationType = viewModel.OperationType,                            
                notShowInputWarnings = viewModel.notShowInputWarnings
            };

            inspection.DraughtSurveyBlocks = new List<DraughtSurveyBlock>
            {
                new DraughtSurveyBlock
                {
                    Inspection = inspection,
                    SurveyType = SurveyType.Initial,
                    Notes = string.Empty
                },
                new DraughtSurveyBlock
                {
                    Inspection = inspection,
                    SurveyType = SurveyType.Final,
                    Notes = string.Empty
                },
            };


            _context.Inspections.Add(inspection);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));            
        }

        // GET: Inspections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var viewModel = new InspectionEditViewModel
            {
                Id = inspection.Id,
                VesselName = inspection.VesselName,
                Port = inspection.Port,
                CompanyReference = inspection.CompanyReference,
                OperationType = inspection.OperationType,
                notShowInputWarnings = inspection.notShowInputWarnings
            };

            return View(viewModel);
        }

        // POST: Inspections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InspectionEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }
            
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var inspection = await _context.Inspections                
                .FindAsync(id);
            if (inspection == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) 
            {
                return RedirectToAction("Login", "Account");
            }

            if (inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }



            bool changed = IsInspectionInputChanged(inspection, viewModel);

            if (changed)
            {
                inspection.VesselName = viewModel.VesselName;
                inspection.Port = viewModel.Port;
                inspection.CompanyReference = viewModel.CompanyReference;
                inspection.OperationType = viewModel.OperationType;
                inspection.notShowInputWarnings = viewModel.notShowInputWarnings;
            }
            
                

            

            await _context.SaveChangesAsync();
            
            return Redirect($"{Url.Action("Details", "Inspections", new { id = viewModel.Id })}#draught-inspection-input");
        }

        // GET: Inspections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspection = await _context.Inspections.FirstOrDefaultAsync(m => m.Id == id);            
            if (inspection == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(inspection);
        }

        // POST: Inspections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }


            _context.Inspections.Remove(inspection);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool IsInspectionInputChanged(Inspection dbValue, InspectionEditViewModel viewModelValue)
        {
            return
                dbValue.VesselName != viewModelValue.VesselName ||
                dbValue.Port != viewModelValue.Port ||
                dbValue.CompanyReference != viewModelValue.CompanyReference ||
                dbValue.OperationType != viewModelValue.OperationType ||
                dbValue.notShowInputWarnings != viewModelValue.notShowInputWarnings;
        }

        //private bool InspectionExists(int id)
        //{
        //    return _context.Inspections.Any(e => e.Id == id);
        //}
    }
}
