using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using static DraughtSurveyWebApp.Utils.Utils;

namespace DraughtSurveyWebApp.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class HydrostaticTablesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HydrostaticTablesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /HydrostaticTables
        public async Task<IActionResult> Index()
        {
            var isAdmin = User.IsInRole("Admin");
            var userId = _userManager.GetUserId(User);

            var query = _context.UserHydrostaticTableHeaders
                .Include(h => h.UserHydrostaticTableRows)
                .AsQueryable();

            if (isAdmin)
            {
                query = query.Include(h => h.ApplicationUser);
            }
            else
            {
                query = query.Where(h => h.ApplicationUserId == userId);
            }

            var headers = await query                
                .OrderByDescending(h => h.Id)
                .ToListAsync();
                      

            return View(headers);
        }

        // GET: /HydrostaticTables/Details/1
        public async Task<IActionResult> Details(int id)
        {
            var isAdmin = User.IsInRole("Admin");
            var userId = _userManager.GetUserId(User);

            var query = _context.UserHydrostaticTableHeaders                
                .Where(h => h.Id == id)
                .Include(h => h.UserHydrostaticTableRows)
                .AsQueryable();

            if (isAdmin)
            {
                query = query.Include(h => h.ApplicationUser);
            }
            else
            {
                query = query.Where(h => h.ApplicationUserId == userId);
            }

            var header = await query
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (header == null)
            {
                return NotFound();
            }

            //var userId = _userManager.GetUserId(User);

            //var header = await _context.UserHydrostaticTableHeaders
            //    .Include(h => h.UserHydrostaticTableRows)
            //    .FirstOrDefaultAsync(h => h.Id == id && h.ApplicationUserId == userId);

            //if (header == null)
            //{
            //    return NotFound();
            //}

            return View(header);
        }

        // GET: /HydrostaticTables/AddRow?headerId=1
        public async Task<IActionResult> AddRow(int headerId)
        {
            var userId = _userManager.GetUserId(User);

            var header = await _context.UserHydrostaticTableHeaders
                .FirstOrDefaultAsync(h => h.Id == headerId && h.ApplicationUserId == userId);

            if (header == null)
            {
                return Unauthorized();
            }

            var model = new UserHydrostaticTableRow
            {
                UserHydrostaticTableHeaderId = headerId                
            };

            return View(model);
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRow(UserHydrostaticTableRow row)
        {
            if (!ModelState.IsValid)
            {
                return View(row);
            }

            var userId = _userManager.GetUserId(User);

            var header = await _context.UserHydrostaticTableHeaders
                .Include(h => h.UserHydrostaticTableRows)
                .FirstOrDefaultAsync(h => h.Id == row.UserHydrostaticTableHeaderId);

            
            if (header == null)
            {
                return NotFound();
            }

            if (header.ApplicationUserId != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }


            if (!row.Draught.HasValue)
            {
                ModelState.AddModelError(nameof(row.Draught), "Please enter a draught value");
                return View(row);
            }

            if (row.Draught.Value == 0)
            {
                ModelState.AddModelError(nameof(row.Draught), "Please enter a valid draught value");
                return View(row);
            }
                       



            bool allRowValuesEntered =
                row.Displacement.HasValue &&
                row.TPC.HasValue &&
                row.LCF.HasValue &&
                row.IsLcfForward.HasValue &&
                row.MTCPlus50.HasValue &&
                row.MTCMinus50.HasValue;

            if (!allRowValuesEntered)
            {
                if (!row.Displacement.HasValue)
                {
                    ModelState.AddModelError(nameof(row.Displacement), "Required");
                }
                if (!row.TPC.HasValue)
                {
                    ModelState.AddModelError(nameof(row.TPC), "Required");
                }
                if (!row.LCF.HasValue)
                {
                    ModelState.AddModelError(nameof(row.IsLcfForward), "Required");
                }
                if (!row.IsLcfForward.HasValue)
                {
                    ModelState.AddModelError(nameof(row.IsLcfForward), "Required");
                }
                if (!row.MTCPlus50.HasValue)
                {
                    ModelState.AddModelError(nameof(row.MTCPlus50), "Required");
                }
                if (!row.MTCMinus50.HasValue)
                {
                    ModelState.AddModelError(nameof(row.MTCMinus50), "Required");
                }
                return View(row);
            }


            var existingRow = await _context.UserHydrostaticTableRows
                .FirstOrDefaultAsync(r => 
                    r.UserHydrostaticTableHeaderId == row.UserHydrostaticTableHeaderId &&
                    r.Draught.HasValue &&                    
                    Math.Abs(r.Draught.Value-row.Draught.Value) < 0.001);

            

            if (existingRow != null)
            {
                existingRow.Displacement = row.Displacement;
                existingRow.TPC = row.TPC;
                existingRow.LCF = row.LCF;
                existingRow.IsLcfForward = row.IsLcfForward;
                existingRow.MTCPlus50 = row.MTCPlus50;
                existingRow.MTCMinus50 = row.MTCMinus50;

                _context.Update(existingRow);
            }
            else
            {
                row.UserHydrostaticTableHeader = header;
                _context.UserHydrostaticTableRows.Add(row);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = row.UserHydrostaticTableHeaderId });
        }

        // GET:
        public async Task<IActionResult> DeleteRow(int id)
        {
            var row = await _context.UserHydrostaticTableRows
                .Include(r => r.UserHydrostaticTableHeader)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (row == null || row?.UserHydrostaticTableHeader?.ApplicationUserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            return View(row);
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRowConfirmed(int id)
        {
            var row = await _context.UserHydrostaticTableRows
                .Include(r => r.UserHydrostaticTableHeader)
                .FirstOrDefaultAsync(r => r.Id == id);
            
            if (row?.UserHydrostaticTableHeader?.ApplicationUserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            if (row == null)
            {
                return NotFound();
            }

            _context.UserHydrostaticTableRows.Remove(row);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = row.UserHydrostaticTableHeaderId });

        }

        // GET:
        public async Task<IActionResult> EditRow(int id)
        {
            var row = await _context.UserHydrostaticTableRows
                .Include(r => r.UserHydrostaticTableHeader)
                .FirstOrDefaultAsync (r => r.Id == id);

            if (row == null || row?.UserHydrostaticTableHeader?.ApplicationUserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            return View(row);
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRow(UserHydrostaticTableRow row)
        {
            if (!ModelState.IsValid)
            {
                return View(row);
            }

            var existing = await _context.UserHydrostaticTableRows
                .Include(r => r.UserHydrostaticTableHeader)
                .FirstOrDefaultAsync(r => r.Id == row.Id);            

            if (existing?.UserHydrostaticTableHeader?.ApplicationUserId != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

            if (existing == null)
            {
                return NotFound();
            }

            existing.Draught = row.Draught;
            existing.Displacement = row.Displacement;
            existing.TPC = row.TPC;
            existing.LCF = row.LCF;
            existing.IsLcfForward = row.IsLcfForward;
            existing.MTCPlus50 = row.MTCPlus50;
            existing.MTCMinus50 = row.MTCMinus50;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = row.UserHydrostaticTableHeaderId});
        }

    }
}
