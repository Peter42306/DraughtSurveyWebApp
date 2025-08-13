using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DraughtSurveyWebApp.Controllers.Admin
{

    [Authorize(Roles = "Admin")]
    [Route("admin/stats")]
    public class StatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class MonthlyRow
        {
            public int Year { get; set; }
            public int Month { get; set; }  // 1..12
            public int ActiveUsers { get; set; }
            public int Registrations { get; set; }
            public int Logins { get; set; }
            public int UniqueLogins { get; set; }
            public int Inspections { get; set; }
            public int HydrostaticTables { get; set; }
            public int ExcelExports { get; set; }
        }

        public class StatsVm
        {
            public int ActiveUsersToday { get; set; }
            public int ActiveUsersThisMonth { get; set; }
            public int RegistrationsThisMonth { get; set; }
            public int RegistrationsTotal { get; set; }
            public int InspectionsThisMonth { get; set; }
            public int InspectionsTotal { get; set; }
            //public int HydroThisMonth { get; set; }
            //public int HydroTotal { get; set; }
            public int ExportsThisMonth { get; set; }
            public int ExportsTotal { get; set; }

            public int LoginsThisMonth { get; set; }
            public int LoginsTotal { get; set; }
            public int UniqueLoginsThisMonth { get; set; }
            public int UniqueLoginsTotal { get; set; }

            public List<MonthlyRow> Monthly { get; set; } = new List<MonthlyRow>();
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;
            var todayStart = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            // active users
            var activeUsersToday = await _context.UserSessions
                .AsNoTracking()
                .Where(u => u.LastSeenUtc >= todayStart)
                .Select(u => u.UserId)
                .Distinct()
                .CountAsync();

            var activeUsersThisMonth = await _context.UserSessions
                .AsNoTracking()
                .Where(u => u.LastSeenUtc >= monthStart)
                .Select(u => u.UserId)
                .Distinct()
                .CountAsync();

            var monthlyActiveUsers = await _context.UserSessions
                .AsNoTracking()
                .GroupBy(s => new { s.LastSeenUtc.Year, s.LastSeenUtc.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Count = g.Select(x => x.UserId).Distinct().Count(),
                })
                .ToListAsync();

            // registrations
            var registrationsThisMonth = await _context.Users
                .AsNoTracking()
                .Where(u => u.CreatedAt >= monthStart)
                .CountAsync();

            var registrationsTotal = await _context.Users
                .AsNoTracking()
                .CountAsync();

            var monthlyRegistrations = await _context.Users
                .AsNoTracking()
                .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            // logins
            var loginsThisMonth = await _context.UserSessions
                .AsNoTracking()
                .Where(s => s.StartedUtc >= monthStart)
                .CountAsync();

            var loginsTotal = await _context.UserSessions
                .AsNoTracking()
                .CountAsync();

            var monthlyLogins = await _context.UserSessions
                .AsNoTracking()
                .GroupBy(s => new { s.StartedUtc.Year, s.StartedUtc.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            // unique logins
            var uniqueLoginsThisMonth = await _context.UserSessions
                .AsNoTracking()
                .Where(s => s.LastSeenUtc >= monthStart)
                .Select(s => s.UserId)
                .Distinct()
                .CountAsync();

            var uniqueLoginsTotal = await _context.UserSessions
                .AsNoTracking()
                .Select(s => s.UserId)
                .Distinct()
                .CountAsync();

            var monthlyUniqueLogins = await _context.UserSessions
                .AsNoTracking()
                .GroupBy(s => new { s.StartedUtc.Year, s.StartedUtc.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Count = g.Select(x => x.UserId).Distinct().Count()
                })
                .ToListAsync();


            // inspections
            var inspectionsThisMonth = await _context.Inspections
                .AsNoTracking()
                .Where(i => i.CreatedAt >= monthStart)
                .CountAsync();

            var inspectionsTotal = await _context.Inspections
                .AsNoTracking()
                .CountAsync();

            var monthlyInspection = await _context.Inspections
                .AsNoTracking()
                .GroupBy(i => new { i.CreatedAt.Year, i.CreatedAt.Month })
                .Select(i => new { i.Key.Year, i.Key.Month, Count = i.Count() })
                .ToListAsync();

            // hydrostatic tables
            //var hydroThisMonth = await _context.UserHydrostaticTableHeaders
            //    .AsNoTracking()
            //    .CountAsync();

            //var hydroTotal = await _context.UserHydrostaticTableHeaders
            //    .AsNoTracking()
            //    .CountAsync();

            // excel exports
            var exportsThisMonth = 0;
            var exportsTotal = 0;
            var monthlyExports = new List<(int Year, int Month, int Count)>();

            if (_context.Set<ExcelExportLog>() is IQueryable<ExcelExportLog> exports)
            {
                exportsThisMonth = await exports
                    .AsNoTracking()
                    .Where(e => e.CreatedUtc >= monthStart)
                    .CountAsync();

                exportsTotal = await exports.AsNoTracking().CountAsync();

                monthlyExports = await exports
                    .AsNoTracking()
                    .GroupBy(e => new { e.CreatedUtc.Year, e.CreatedUtc.Month })
                    .Select(g => new ValueTuple<int,int,int>(g.Key.Year,g.Key.Month,g.Count()))
                    .ToListAsync();                
            }

            var keys = monthlyActiveUsers
                    .Select(x => (x.Year, x.Month))
                    .Concat(monthlyRegistrations.Select(x => (x.Year, x.Month)))
                    .Concat(monthlyInspection.Select(x => (x.Year, x.Month)))
                    .Concat(monthlyExports.Select(x => (x.Year, x.Month)))
                    .Concat(monthlyLogins.Select(x => (x.Year, x.Month)))
                    .Concat(monthlyUniqueLogins.Select(x => (x.Year, x.Month)))
                    .Distinct()
                    .OrderBy(x => x.Year)
                        .ThenBy(x => x.Month)
                    .ToList();

            var monthly = new List<MonthlyRow>();
            foreach (var k in keys)
            {
                monthly.Add(new MonthlyRow
                {
                    Year = k.Year,
                    Month = k.Month,
                    ActiveUsers = monthlyActiveUsers.FirstOrDefault(x => x.Year == k.Year && x.Month == k.Month)?.Count ?? 0,
                    Registrations = monthlyRegistrations.FirstOrDefault(x => x.Year == k.Year && x.Month == k.Month)?.Count ?? 0,
                    Inspections = monthlyInspection.FirstOrDefault(x => x.Year == k.Year && x.Month == k.Month)?.Count ?? 0,
                    ExcelExports = monthlyExports.FirstOrDefault(x => x.Year == k.Year && x.Month == k.Month).Count,
                    Logins = monthlyLogins.FirstOrDefault(x => x.Year == k.Year && x.Month == k.Month)?.Count ?? 0,
                    UniqueLogins = monthlyUniqueLogins.FirstOrDefault(x => x.Year == k.Year && x.Month == k.Month)?.Count ?? 0,
                });
            }

            var vm = new StatsVm
            {
                ActiveUsersToday = activeUsersToday,
                ActiveUsersThisMonth = activeUsersThisMonth,
                RegistrationsThisMonth = registrationsThisMonth,
                RegistrationsTotal = registrationsTotal,
                InspectionsThisMonth = inspectionsThisMonth,
                InspectionsTotal = inspectionsTotal,
                ExportsThisMonth = exportsThisMonth,
                ExportsTotal = exportsTotal,
                LoginsThisMonth = loginsThisMonth,
                LoginsTotal = loginsTotal,
                UniqueLoginsThisMonth = uniqueLoginsThisMonth,
                UniqueLoginsTotal = uniqueLoginsTotal,
                Monthly = monthly
            };

            return View(vm);

        }



    }
}
