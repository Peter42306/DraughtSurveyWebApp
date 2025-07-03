using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Services;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DraughtSurveyWebApp.Controllers
{
    public class HydrostaticInputController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SurveyCalculationsService _surveyCalculationsService;

        public HydrostaticInputController(ApplicationDbContext context, SurveyCalculationsService surveyCalculationsService)
        {
            _context = context;
            _surveyCalculationsService = surveyCalculationsService;
        }

        // GET: HydrostaticInput/Edit?draughtSurveyBlockId=1
        public async Task<IActionResult> Edit(int draughtSurveyBlockId)
        {
            var draughtSurveyBlock = await _context.DraughtSurveyBlocks
                .Include(h => h.HydrostaticInput)
                .Include(r => r.DraughtsResults)
                .FirstOrDefaultAsync(b => b.Id == draughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }

            var inputs = draughtSurveyBlock.HydrostaticInput;
            var results = draughtSurveyBlock.DraughtsResults;




            //double? meanAdjustedDraught = results?.MeanAdjustedDraught;

            //double? DraughtAboveMTCPlus50 = null;
            //double? DraughtBelowMTC
            //if (meanAdjustedDraught.HasValue)
            //{
            //    DraughtAboveMTCPlus50 = _surveyCalculationsService.CalculateDraughtForMTCPlus50(meanAdjustedDraught.Value);
            //}
            

            var viewModel = new HydrostaticInputViewModel
            {
                DraughtSurveyBlockId = draughtSurveyBlock.Id,
                InspectionId = draughtSurveyBlock.InspectionId,

                DraughtBelow = inputs?.DraughtBelow,
                DraughtAbove = inputs?.DraughtAbove,
                MeanAdjustedDraught = results?.MeanAdjustedDraught,

                
            };

            return View(viewModel);
        }




        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
