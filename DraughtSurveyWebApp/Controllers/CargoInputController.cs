using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace DraughtSurveyWebApp.Controllers
{
    public class CargoInputController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CargoInputController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CargoInput/Create?inspectionId=5
        public IActionResult Create(int inspectionId)
        {
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

            var cargo = new CargoInput
            {
                InspectionId = viewModel.InspectionId,
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
            var cargo = await _context.CargoInputs
                .FirstOrDefaultAsync(c => c.InspectionId == inspectionId);

            if (cargo == null)
            {
                return NotFound();
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

            var cargo = await _context.CargoInputs
                .FirstOrDefaultAsync(c => c.InspectionId == inspectionId);

            if (cargo == null)
            {
                return NotFound();
            }

            cargo.CargoName = viewModel.CargoName;
            cargo.DeclaredWeight = viewModel.DeclaredWeight;
            cargo.LoadingTerminal = viewModel.LoadingTerminal;
            cargo.BerthNumber = viewModel.BerthNumber;

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Inspections", new { id = cargo.InspectionId });
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
