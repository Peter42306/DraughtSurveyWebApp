using Microsoft.AspNetCore.Mvc;

namespace DraughtSurveyWebApp.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
