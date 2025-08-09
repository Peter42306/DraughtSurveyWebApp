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
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using ClosedXML.Excel;

namespace DraughtSurveyWebApp.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class InspectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SurveyCalculationsService _surveyCalculationsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InspectionsController(
            ApplicationDbContext context,
            SurveyCalculationsService surveyCalculationsService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _surveyCalculationsService = surveyCalculationsService;
            _userManager = userManager;
            _webHostEnvironment=webHostEnvironment;
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
                    .Include(i => i.ApplicationUser)
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
                .Include(i => i.CargoResult)
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
                notShowInputWarnings = viewModel.notShowInputWarnings,
                notApplyAutoFillingHydrostatics = viewModel.notApplyAutoFillingHydrostatics

            };

            inspection.DraughtSurveyBlocks = new List<DraughtSurveyBlock>
            {
                new DraughtSurveyBlock
                {
                    InspectionId = inspection.Id,
                    Inspection = inspection,
                    SurveyType = SurveyType.Initial,
                    Notes = string.Empty
                },
                new DraughtSurveyBlock
                {
                    InspectionId = inspection.Id,
                    Inspection = inspection,
                    SurveyType = SurveyType.Final,
                    Notes = string.Empty
                },
            };

            inspection.CargoResult = new CargoResult
            {
                Inspection = inspection,
                InspectionId = inspection.Id,
                CargoByDraughtSurvey = null,
                DifferenceWithBL_Mt = null,
                DifferenceWithBL_Percents = null,
                DifferenceWithSDWT_Percents = null
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
                notShowInputWarnings = inspection.notShowInputWarnings,
                notApplyAutoFillingHydrostatics = inspection.notApplyAutoFillingHydrostatics
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
                inspection.notApplyAutoFillingHydrostatics = viewModel.notApplyAutoFillingHydrostatics;
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inspectionId"></param>
        /// <param name="excelTemplateId"></param>
        /// <returns></returns>
        public async Task<IActionResult> ExportExcel(int inspectionId, int excelTemplateId)
        {
            var inspection = await _context.Inspections
                .AsNoTracking()
                .Include(i => i.VesselInput)
                .Include(i => i.CargoInput)
                .Include(i => i.CargoResult)
                .Include(i => i.DraughtSurveyBlocks)
                .FirstOrDefaultAsync(i => i.Id == inspectionId);
            if (inspection == null)
            {
                return NotFound();
            }

            var initial = inspection.DraughtSurveyBlocks
                .FirstOrDefault(b => b.SurveyType == SurveyType.Initial);

            var final = inspection.DraughtSurveyBlocks
                .FirstOrDefault(b => b.SurveyType == SurveyType.Final);

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");
            if (!isAdmin && inspection?.ApplicationUserId != userId)
            {
                return Forbid();
            }

            var excelTemplate = await _context.ExcelTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == excelTemplateId);
            if (excelTemplate == null)
            {
                return NotFound();
            }

            if (!isAdmin && !(excelTemplate.IsPublic || excelTemplate.OwnerId == userId))
            {
                return Forbid();
            }

            var excelTemplatePath = Path.Combine(
                _webHostEnvironment.ContentRootPath,
                excelTemplate.FilePath.Replace('/', Path.DirectorySeparatorChar)
            );

            if (!System.IO.File.Exists(excelTemplatePath))
            {
                return NotFound("Template file not found.");
            }

            var map = ExcelExportMapper.CreateMap(
                inspection,
                initial,
                final
            );

            using var wb = new XLWorkbook(excelTemplatePath);
            ExcelTemplateFiller.FillExcelByName( wb, map );

            using var ms = new MemoryStream();
            wb.SaveAs( ms );
            ms.Position = 0;

            const string mime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var downloadName = $"{(string.IsNullOrWhiteSpace(excelTemplate.Name) ? "Report" : excelTemplate.Name)}_filled.xlsx";
            return File(ms, mime, downloadName);
        }

        
        public async Task<IActionResult> ChooseExcelTemplate(int inspectionId)
        {
            var inspection = await _context.Inspections
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == inspectionId);
            if (inspection == null) 
            {
                return NotFound(); 
            }

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            var queue = _context.ExcelTemplates.AsNoTracking();
            if (!isAdmin)
            {
                queue = queue.Where(t => t.IsPublic || t.OwnerId == userId);
            }

            var templates = await queue
                .OrderBy(t => t.Name)
                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name})
                .ToListAsync();

            var viewModel = new ExcelTemplateUserSelectTemplate
            {
                InspectionId = inspectionId,
                ExcelTemplates = templates,
                SelectedTemplateId = templates.FirstOrDefault() is { } first 
                    ? int.Parse(first.Value) 
                    : (int?)null
            };

            return View(viewModel);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbValue"></param>
        /// <param name="viewModelValue"></param>
        /// <returns></returns>
        private bool IsInspectionInputChanged(Inspection dbValue, InspectionEditViewModel viewModelValue)
        {
            return
                dbValue.VesselName != viewModelValue.VesselName ||
                dbValue.Port != viewModelValue.Port ||
                dbValue.CompanyReference != viewModelValue.CompanyReference ||
                dbValue.OperationType != viewModelValue.OperationType ||
                dbValue.notShowInputWarnings != viewModelValue.notShowInputWarnings ||
                dbValue.notApplyAutoFillingHydrostatics != viewModelValue.notApplyAutoFillingHydrostatics;
        }

        //private bool InspectionExists(int id)
        //{
        //    return _context.Inspections.Any(e => e.Id == id);
        //}
    }
}
