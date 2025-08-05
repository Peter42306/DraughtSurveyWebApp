using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var userId = _userManager.GetUserId(User);

            var header = await _context.UserHydrostaticTableHeaders
                .Where(h => h.ApplicationUserId == userId)
                .Include(h => h.UserHydrostaticTableRows)
                .ToListAsync();

            return View(header);
        }

        // GET: /HydrostaticTables/Details/1
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);

            var header = await _context.UserHydrostaticTableHeaders
                .Include(h => h.UserHydrostaticTableRows)
                .FirstOrDefaultAsync(h => h.Id == id && h.ApplicationUserId == userId);

            if (header == null)
            {
                return NotFound();
            }

            return View(header);
        }

        // GET: 
        public IActionResult AddRow(int headerId)
        {
            var model = new UserHydrostaticTableRow
            {
                UserHydrostaticTableHeaderId = headerId                
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRow(UserHydrostaticTableRow row)
        {
            if (!ModelState.IsValid)
            {
                return View(row);
            }

            var header = await _context.UserHydrostaticTableHeaders
                .FirstOrDefaultAsync(h => h.Id == row.UserHydrostaticTableHeaderId);

            if (header == null || header.ApplicationUserId != _userManager.GetUserId(User))
            {
                return Unauthorized();
            }

            row.UserHydrostaticTableHeader = header;
            _context.UserHydrostaticTableRows.Add(row);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = row.UserHydrostaticTableHeaderId });
        }




        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
