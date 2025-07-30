using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DraughtSurveyWebApp.Areas.Identity.Pages.Account
{
    public class RegisterConfirmationModel : PageModel
    {
        public string Email { get; set; } = string.Empty;

        public void OnGet(string? email = null)
        {
            Email = email ?? string.Empty;
        }
    }
}
