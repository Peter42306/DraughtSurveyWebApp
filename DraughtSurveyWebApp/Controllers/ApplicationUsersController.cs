using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.Data;
using Microsoft.EntityFrameworkCore;

namespace DraughtSurveyWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ApplicationUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public ApplicationUsersController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
        }
        
        

        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;
            var onlineThreshold = now.AddMinutes(-5);

            // Пользователи + их последняя активность (LEFT JOIN)
            var baseQuery = _userManager.Users
                .AsNoTracking()
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.IsActive,
                    u.CreatedAt,
                    u.LoginCount,
                    u.AdminNote,

                    // Последняя активность (Max по пользователю)
                    LastSeenUtc = _applicationDbContext.UserSessions
                        .Where(s => s.UserId == u.Id)
                        .Select(s => (DateTime?)s.LastSeenUtc)
                        .Max(),

                    // Онлайн сейчас? (Any за порог)
                    IsOnline = _applicationDbContext.UserSessions
                        .Where(s => s.UserId == u.Id && s.LastSeenUtc >= onlineThreshold)
                        .Any(),

                    // Поля ПОСЛЕДНЕЙ сессии (та, у которой максимальный LastSeenUtc)
                    LastStartedUtc = _applicationDbContext.UserSessions
                        .Where(s => s.UserId ==u.Id)
                        .OrderByDescending(s => s.LastSeenUtc)
                        .Select(s => (DateTime?)s.StartedUtc)
                        .FirstOrDefault(),


                    LastIp = _applicationDbContext.UserSessions
                        .Where(s => s.UserId ==u.Id)
                        .OrderByDescending (s => s.LastSeenUtc)
                        .Select(s => s.Ip)
                        .FirstOrDefault(),


                    LastUserAgent = _applicationDbContext.UserSessions
                        .Where(s => s.UserId ==u.Id)
                        .OrderByDescending(s => s.LastSeenUtc)
                        .Select(s => s.UserAgent)
                        .FirstOrDefault()
                });                

            // Сортируем в БД, потом мапим в UserRow
            var model = await baseQuery
                .OrderByDescending(x => x.IsOnline)
                .ThenByDescending(x => x.LastSeenUtc)
                .ThenByDescending(x => x.LoginCount)
                .Select(x => new UserRow(
                    x.Id,
                    x.Email,
                    x.IsActive,
                    x.CreatedAt,
                    x.LoginCount,
                    x.LastSeenUtc,
                    x.IsOnline,
                    x.AdminNote,
                    x.LastStartedUtc,
                    x.LastIp,
                    x.LastUserAgent
                    ))
                .ToListAsync();
            

            return View(model);

        }



        //public IActionResult Index()
        //{
        //    var users = _userManager.Users.ToList();
        //    return View(users);
        //}     

        

        /// <summary>        
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isActive"></param>
        /// <param name="isConfirmed"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus()
        {
            //TempData["Error"] = $"Received: isActive = {Request.Form["isActive"]}";

            if (string.IsNullOrWhiteSpace(Request.Form["id"]))
            {
                TempData["Error"] = "User ID is required";
                return RedirectToAction(nameof(Index));
            }
            
            var id = Request.Form["id"].ToString();
            var isActive = Request.Form["isActive"].Contains("true");
            var adminNote = Request.Form["adminNote"];

            var currentUser = await _userManager.GetUserAsync(User);            

            // Защита: админ не может отключить сам себя
            if (id == currentUser?.Id && !isActive)
            {
                TempData["Error"] = "You cannot deactivate your own admin account";
                return RedirectToAction(nameof(Index));
            }


            
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                user.IsActive = isActive;
                user.AdminNote = adminNote;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                }                
            }
            return RedirectToAction(nameof(Index));
        }

        
        public class UserRow
        {
            public string Id { get; set; }
            public string? Email { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? LastSeenUtc { get; set; }
            public int LoginCount { get; set; }
            public bool IsOnLine { get; set; }
            public string? AdminNote { get; set; }

            public DateTime? LastStartedUtc { get; set; }
            public string? LastIp { get; set; }
            public string? LastUserAgent { get; set; }


            public UserRow(
                string id,
                string? email,
                bool isActive, 
                DateTime createdAt, 
                int loginCount, 
                DateTime? lastSeenUtc, 
                bool isOnLine, 
                string? adminNote,

                DateTime? lastStartedUtc,
                string? lastIp,
                string? lastUserAgent
                )
            {
                Id = id;
                Email = email;
                IsActive = isActive;
                CreatedAt = createdAt;
                LoginCount = loginCount;
                LastSeenUtc = lastSeenUtc;
                IsOnLine = isOnLine;
                AdminNote = adminNote;
                LastStartedUtc = lastStartedUtc;
                LastIp = lastIp;
                LastUserAgent = lastUserAgent;
            }
        }

    }
}
