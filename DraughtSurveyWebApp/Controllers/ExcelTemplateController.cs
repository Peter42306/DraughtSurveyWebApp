using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DraughtSurveyWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ExcelTemplateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ExcelTemplateController> _logger;

        public ExcelTemplateController(
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager,
            ILogger<ExcelTemplateController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _logger=logger;
        }

        // List of all Excel templates
        
        public async Task<IActionResult> Index()
        {
            var templates = await _context.ExcelTemplates
                .Include(t => t.Owner)
                .OrderByDescending(t => t.CreatedAtUtc)
                .ToListAsync();

            return View(templates);
        }

        // GET:        
        public async Task<IActionResult> Create()
        {
            var model = new ExcelTemplateCreateViewModel
            {
                Owners = await _context.Users
                .OrderBy(u => u.Email)
                .Select(u => new SelectListItem { Value = u.Id, Text = u.Email})
                .ToListAsync()
            };
            return View(model);
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]        
        [RequestFormLimits(MultipartBodyLengthLimit =50_000_000)]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> Create(ExcelTemplateCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Owners = await _context.Users
                    .OrderBy(u => u.Email)
                    .Select(u => new SelectListItem { Value = u.Id, Text = u.Email})
                    .ToListAsync();

                return View(viewModel);
            }

            if (viewModel == null || viewModel.File == null || viewModel?.File?.Length == 0)
            {
                ModelState.AddModelError(nameof(viewModel.File), "Please upload a file");
                return View(viewModel);
            }            

            if (string.IsNullOrWhiteSpace(viewModel?.File.FileName))
            {
                ModelState.AddModelError(nameof(viewModel.File), "Please upload a file");

                if (viewModel != null)
                {
                    viewModel.Owners = await _context.Users
                        .OrderBy(u => u.Email)
                        .Select(u => new SelectListItem { Value = u.Id, Text = u.Email })
                        .ToListAsync();
                }                    

                return View(viewModel);
            }

            
            if (!viewModel.IsPublic && string.IsNullOrWhiteSpace(viewModel.OwnerId))
            {
                ModelState.AddModelError(nameof(viewModel.OwnerId), "Owner is required for a private template");

                viewModel.Owners = await _context.Users
                    .OrderBy(u => u.Email)
                    .Select(u => new SelectListItem { Value = u.Id, Text = u.Email })
                    .ToListAsync();

                return View(viewModel);
            }

            if (!viewModel.IsPublic)
            {
                var ownerExist = await _context.Users.AnyAsync(u => u.Id == viewModel.OwnerId);
                if (!ownerExist)
                {
                    ModelState.AddModelError(nameof(viewModel.OwnerId), "Selected owner does not exist");

                    viewModel.Owners = await _context.Users
                    .OrderBy(u => u.Email)
                    .Select(u => new SelectListItem { Value = u.Id, Text = u.Email })
                    .ToListAsync();

                    return View(viewModel);
                }
            }

            var extension = Path.GetExtension(viewModel.File.FileName).ToLowerInvariant();
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".xlsx", ".xls", ".xlsm"
            };

            if (!allowed.Contains(extension))
            {
                ModelState.AddModelError(nameof(viewModel.File), "Only Excel files (.xlsx, .xls, .xlsm) are allowed");
                return View(viewModel);
            }

            // Create folder
            var storageRoot = Path.Combine(_webHostEnvironment.ContentRootPath, "appdata", "templates");
            Directory.CreateDirectory(storageRoot);

            // Create file name
            var fileNameStored = $"{Guid.NewGuid():N}{extension}";            
            var fullPath = Path.Combine(storageRoot, fileNameStored);
            var relativePath = $"appdata/templates/{fileNameStored}";
            var tempPath = fullPath + ".tmp";

            try
            {
                

                await using (var fs =
                    new FileStream(
                        tempPath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None,
                        81920,
                        useAsync: true))
                {
                    await viewModel.File.CopyToAsync(fs, HttpContext.RequestAborted);
                }
                System.IO.File.Move(tempPath, fullPath);

                var contentType = string.IsNullOrWhiteSpace(viewModel.File.ContentType)
                    ? extension switch
                    {
                        ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        ".xlsm" => "application/vnd.ms-excel.sheet.macroEnabled.12",
                        ".xls" => "application/vnd.ms-excel",
                        _ => "application/octet-stream"
                    }
                    : viewModel.File.ContentType;

                var template = new ExcelTemplate
                {
                    Name = viewModel.Name?.Trim() ?? "",
                    FilePath = relativePath,
                    IsPublic = string.IsNullOrWhiteSpace(viewModel.OwnerId),
                    OwnerId = string.IsNullOrWhiteSpace(viewModel.OwnerId) ? null : viewModel.OwnerId,
                    OriginalFileName = viewModel.File.FileName,
                    ContentType = contentType,
                    FileSizeBytes = viewModel.File.Length,
                    CreatedAtUtc = DateTime.UtcNow
                };

                _context.ExcelTemplates.Add(template);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
                
            }
            catch (DbUpdateException)
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                if (System.IO.File.Exists(tempPath))
                {
                    System.IO.File.Delete(tempPath);
                }

                ModelState.AddModelError("", "Database error. Please try again");

                viewModel.Owners = await _context.Users
                    .OrderBy(u => u.Email)
                    .Select(u => new SelectListItem { Value = u.Id, Text = u.Email })
                    .ToListAsync();

                return View(viewModel);
            }
            catch(IOException)
            {
                ModelState.AddModelError("", "File saving error. Please try again");

                viewModel.Owners = await _context.Users
                    .OrderBy(u => u.Email)
                    .Select(u => new SelectListItem { Value = u.Id, Text = u.Email })
                    .ToListAsync();

                return View(viewModel);
            }
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError("", "No write access to the upload directory");

                viewModel.Owners = await _context.Users
                    .OrderBy(u => u.Email)
                    .Select(u => new SelectListItem { Value = u.Id, Text = u.Email })
                    .ToListAsync();

                return View(viewModel);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unexpected error. Please try again");

                viewModel.Owners = await _context.Users
                    .OrderBy(u => u.Email)
                    .Select(u => new SelectListItem { Value = u.Id, Text = u.Email })
                    .ToListAsync();

                return View(viewModel);
            }

        }
        
        public async Task<IActionResult> SelectTemplate()
        {
            var userId = _userManager.GetUserId(User);

            var templates = await _context.ExcelTemplates                
                .Where(t => t.IsPublic || t.OwnerId == userId)
                .OrderBy(t => t.Name)
                .ToListAsync();

            return View(templates);
        }


        public async Task<IActionResult> Download(int id)
        {
            var template = await _context.ExcelTemplates.FindAsync(id);
            if (template == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && !(template.IsPublic || template.OwnerId == userId))
            {
                return Forbid();
            }

            var fullPath = 
                Path.Combine(
                    _webHostEnvironment.ContentRootPath, 
                    template.FilePath.Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            var stream = new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                81920,
                useAsync: true);

            var contentType = string.IsNullOrWhiteSpace(template.ContentType) ? "application/octet-stream" : template.ContentType;

            var downloadName = string.IsNullOrWhiteSpace(template.OriginalFileName) ? Path.GetFileName(fullPath) : template.OriginalFileName;

            //var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);


            return File(stream, contentType, downloadName, enableRangeProcessing: true);
        }


        // GET:        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var template = await _context.ExcelTemplates
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            return View(template);
        }

        
        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var template = await _context.ExcelTemplates.FirstOrDefaultAsync(x => x.Id == id);
            if (template == null)
            {
                return NotFound();
            }
                        
            var fullPath = Path.Combine(
                _webHostEnvironment.ContentRootPath,
                template.FilePath.Replace('/', Path.DirectorySeparatorChar));

            try
            {
                _context.ExcelTemplates.Remove(template);
                await _context.SaveChangesAsync();

                try
                {
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"File from {fullPath} was not deleted", fullPath);
                }

                TempData["Success"] = "Template deleted";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Cannot delete template dur to related data";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Unexpected error while deleting template";
                return RedirectToAction(nameof(Index));
            }

        }

        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
