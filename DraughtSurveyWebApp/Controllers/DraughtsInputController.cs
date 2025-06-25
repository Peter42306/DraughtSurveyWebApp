using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Services;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DraughtSurveyWebApp.Controllers
{
    public class DraughtsInputController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SurveyCalculationsService _surveyCalculationsService;

        public DraughtsInputController(ApplicationDbContext context, SurveyCalculationsService surveyCalculationsService)
        {
            _context = context;
            _surveyCalculationsService = surveyCalculationsService;
        }

        // GET: DraughtsInput/Edit?draughtSurveyBlockId=5
        public async Task<IActionResult> Edit(int draughtSurveyBlockId)
        {
            var draughtSurveyBlock = await _context.DraughtSurveyBlocks
                .Include(b => b.DraughtsInput)
                .Include(r => r.DraughtsResults)
                .FirstOrDefaultAsync(b => b.Id == draughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }
            
            var inputs = draughtSurveyBlock.DraughtsInput;
            var results = draughtSurveyBlock.DraughtsResults;


            var viewModel = new DraughtsInputViewModel
            {
                DraughtSurveyBlockId = draughtSurveyBlock.Id,
                InspectionId = draughtSurveyBlock.InspectionId,

                DraughtFwdPS = inputs?.DraughtFwdPS,
                DraughtFwdSS = inputs?.DraughtFwdSS,
                DraughtMidPS = inputs?.DraughtMidPS,
                DraughtMidSS = inputs?.DraughtMidSS,
                DraughtAftPS = inputs?.DraughtAftPS,
                DraughtAftSS = inputs?.DraughtAftSS,

                DistanceFwd = inputs?.DistanceFwd,
                DistanceMid = inputs?.DistanceMid,
                DistanceAft = inputs?.DistanceAft,

                KeelCorrection = inputs?.KeelCorrection,
                SeaWaterDensity = inputs?.SeaWaterDensity,

                DraughtMeanFwd = results?.DraughtMeanFwd,
                DraughtMeanMid = results?.DraughtMeanMid,
                DraughtMeanAft = results?.DraughtMeanAft,

                DraughtCorrectionFwd = results?.DraughtCorrectionFwd,
                DraughtCorrectionMid = results?.DraughtCorrectionMid,
                DraughtCorrectionAft = results?.DraughtCorrectionAft,

                DraughtCorrectedFwd = results?.DraughtCorrectedFwd,
                DraughtCorrectedMid = results?.DraughtCorrectedMid,
                DraughtCorrectedAft = results?.DraughtCorrectedAft,

                TrimApparent = results?.TrimApparent,
                TrimCorrected = results?.TrimCorrected,
                Heel = results?.Heel,
                HoggingSagging = results?.HoggingSagging,
                MeanAdjustedDraught = results?.MeanAdjustedDraught                
            };

            return View(viewModel);
        }

        // POST: DraughtsInput/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DraughtsInputViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var draughtSurveyBlock = await _context.DraughtSurveyBlocks
                .Include(b => b.DraughtsInput)
                .Include(b => b.Inspection)
                    .ThenInclude(i => i.VesselInput)
                .FirstOrDefaultAsync(b => b.Id == viewModel.DraughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }

            if (draughtSurveyBlock.DraughtsInput == null)
            {
                draughtSurveyBlock.DraughtsInput = new Models.DraughtsInput
                {
                    DraughtSurveyBlockId = viewModel.DraughtSurveyBlockId
                };

                _context.DraughtsInputs.Add(draughtSurveyBlock.DraughtsInput);
            }

            var input = draughtSurveyBlock.DraughtsInput;

            input.DraughtFwdPS = viewModel.DraughtFwdPS ?? 0;
            input.DraughtFwdSS = viewModel.DraughtFwdSS ?? 0;
            input.DraughtMidPS = viewModel.DraughtMidPS ?? 0;
            input.DraughtMidSS = viewModel.DraughtMidSS ?? 0;
            input.DraughtAftPS = viewModel.DraughtAftPS ?? 0;
            input.DraughtAftSS = viewModel.DraughtAftSS ?? 0;

            input.DistanceFwd = viewModel.DistanceFwd ?? 0;
            input.DistanceMid = viewModel.DistanceMid ?? 0;
            input.DistanceAft = viewModel.DistanceAft ?? 0;

            input.SeaWaterDensity = viewModel.SeaWaterDensity ?? 0;
            input.KeelCorrection = viewModel.KeelCorrection ?? 0;

            
            double fwdMean = _surveyCalculationsService.CalculateApparentMean(input.DraughtFwdPS, input.DraughtFwdSS);
            double midMean = _surveyCalculationsService.CalculateApparentMean(input.DraughtMidPS, input.DraughtMidSS);
            double aftMean = _surveyCalculationsService.CalculateApparentMean(input.DraughtAftPS, input.DraughtAftSS);

            double trimApparent = _surveyCalculationsService.CalculateTrim(fwdMean, aftMean);
            

            double bm = draughtSurveyBlock.Inspection?.VesselInput?.BM ?? 0;
            double lbp = draughtSurveyBlock.Inspection?.VesselInput?.LBP ?? 0;            

            double lbd = lbp + input.DistanceFwd - input.DistanceAft;            

            double draughtCorrectionFwd = _surveyCalculationsService.CalculateTrimCorrection(input.DistanceFwd, trimApparent, lbd);
            double draughtCorrectionMid = _surveyCalculationsService.CalculateTrimCorrection(input.DistanceMid, trimApparent, lbd);
            double draughtCorrectionAft = _surveyCalculationsService.CalculateTrimCorrection(input.DistanceAft, trimApparent, lbd);

            double draughtCorrectedFwd = _surveyCalculationsService.CalculateCorrectedDraught(fwdMean, draughtCorrectionFwd);
            double draughtCorrectedMid = _surveyCalculationsService.CalculateCorrectedDraught(midMean, draughtCorrectionMid);
            double draughtCorrectedAft = _surveyCalculationsService.CalculateCorrectedDraught(aftMean, draughtCorrectionAft);

            double trimCorrected = _surveyCalculationsService.CalculateTrim(draughtCorrectedFwd, draughtCorrectedAft);
            double heel = _surveyCalculationsService.CalculateHeel(input.DraughtMidPS, input.DraughtMidSS, bm);
            double hogSag = _surveyCalculationsService.CalculateHoggingSagging(draughtCorrectedFwd, draughtCorrectedMid, draughtCorrectedAft);
            double meanAdjustedDraught = _surveyCalculationsService.CalculateMeanOfMean(draughtCorrectedFwd, draughtCorrectedMid, draughtCorrectedAft);




            //double hoggingSagging = _surveyCalculationsService.CalculateHoggingSagging()

            //double fwdCorrection = _surveyCalculationsService.CalculateTrimCorrection


            var results = await _context.DraughtsResults
                .FirstOrDefaultAsync(r => r.DraughtSurveyBlockId == draughtSurveyBlock.Id);

            if (results == null)
            {
                results = new Models.DraughtsResults
                {
                    DraughtSurveyBlockId = draughtSurveyBlock.Id,
                    DraughtSurveyBlock = draughtSurveyBlock
                };

                _context.DraughtsResults.Add(results);
            }

            results.DraughtMeanFwd = fwdMean;
            results.DraughtMeanMid = midMean;
            results.DraughtMeanAft = aftMean;

            results.TrimApparent = trimApparent;

            results.DraughtCorrectionFwd = draughtCorrectionFwd;
            results.DraughtCorrectionMid = draughtCorrectionMid;
            results.DraughtCorrectionAft = draughtCorrectionAft;

            results.DraughtCorrectedFwd = draughtCorrectedFwd;
            results.DraughtCorrectedMid = draughtCorrectedMid;
            results.DraughtCorrectedAft = draughtCorrectedAft;

            results.TrimCorrected = trimCorrected;
            results.Heel = heel;
            results.HoggingSagging = hogSag;
            results.MeanAdjustedDraught = meanAdjustedDraught;

            await _context.SaveChangesAsync();

            //return View(viewModel);
            return RedirectToAction("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId });
        }


        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
