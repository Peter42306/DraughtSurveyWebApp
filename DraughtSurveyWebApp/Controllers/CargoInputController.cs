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

        public CargoInputController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;            
            _userManager = userManager;
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
            };

            _context.CargoInputs.Add(cargo);
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
                BerthNumber= cargo.BerthNumber
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

            cargo.CargoName = viewModel.CargoName;
            cargo.DeclaredWeight = viewModel.DeclaredWeight;
            cargo.LoadingTerminal = viewModel.LoadingTerminal;
            cargo.BerthNumber = viewModel.BerthNumber;

            await _context.SaveChangesAsync();

            //return RedirectToAction("Details", "Inspections", new { id = cargo.InspectionId });
            return Redirect($"{Url.Action("Details", "Inspections", new { id = cargo.InspectionId })}#draught-cargo-input");
        }       
               
    }
}
