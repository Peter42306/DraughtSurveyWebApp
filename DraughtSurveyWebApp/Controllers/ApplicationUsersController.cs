using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DraughtSurveyWebApp.Models;

namespace DraughtSurveyWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ApplicationUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }     

        

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

            var id = Request.Form["id"];
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

        
    }
}
