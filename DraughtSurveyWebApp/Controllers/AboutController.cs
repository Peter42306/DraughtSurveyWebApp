using Microsoft.AspNetCore.Mvc;

namespace DraughtSurveyWebApp.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
