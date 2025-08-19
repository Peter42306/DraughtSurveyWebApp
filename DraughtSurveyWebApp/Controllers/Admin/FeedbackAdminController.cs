using DraughtSurveyWebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DraughtSurveyWebApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class FeedbackAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbackAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await _context.FeedbackTickets
                .OrderByDescending(x => x.CreatedUtc)
                .ToListAsync();

            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _context.FeedbackTickets
                .Include(u => u.ApplicationUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavesNote(int id, string? adminNote)
        {
            var ticket = await _context.FeedbackTickets                
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                NotFound();
            }

            if (ticket != null)
            {
                ticket.AdminNotes = string.IsNullOrWhiteSpace(adminNote) ? null : adminNote.Trim();
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
