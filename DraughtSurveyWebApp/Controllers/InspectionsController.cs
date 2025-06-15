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

namespace DraughtSurveyWebApp.Controllers
{
    public class InspectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InspectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inspections
        public async Task<IActionResult> Index()
        {
            //Console.WriteLine("Entered Inspection Index");
            //return Content("REACHED GET Create");

            //var inspections = await _context.Inspections.ToListAsync();

            var inspections = await _context.Inspections
                .OrderByDescending(i => i.Id)
                .ToListAsync();

            return View(inspections);
        }

        // GET: Inspections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var inspection = await _context.Inspections
                .Include(i => i.VesselInput)
                .Include(i => i.CargoInput)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (id == null)
            {
                return NotFound();
            }
            
            if (inspection == null)
            {
                return NotFound();
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

            var inspection = new Inspection
            {
                VesselName = viewModel.VesselName,
                Port = viewModel.Port,
                CompanyReference = viewModel.CompanyReference,
                OperationType = viewModel.OperationType
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

            var viewModel = new InspectionEditViewModel
            {
                Id = inspection.Id,
                VesselName = inspection.VesselName,
                Port = inspection.Port,
                CompanyReference = inspection.CompanyReference,
                OperationType = inspection.OperationType
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

            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection == null)
            {
                return NotFound();
            }

            inspection.VesselName = viewModel.VesselName;
            inspection.Port = viewModel.Port;
            inspection.CompanyReference = viewModel.CompanyReference;
            inspection.OperationType = viewModel.OperationType;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));            
        }

        // GET: Inspections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspection = await _context.Inspections
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspection == null)
            {
                return NotFound();
            }

            return View(inspection);
        }

        // POST: Inspections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspection = await _context.Inspections.FindAsync(id);
            if (inspection != null)
            {
                _context.Inspections.Remove(inspection);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InspectionExists(int id)
        {
            return _context.Inspections.Any(e => e.Id == id);
        }
    }
}
