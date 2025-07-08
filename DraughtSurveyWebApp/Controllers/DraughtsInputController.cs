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

                IsFwdDistancetoFwd = inputs?.isFwdDistancetoFwd ?? false,
                IsMidDistanceToFwd = inputs?.isMidDistanceToFwd ?? false,
                IsAftDistanceToFwd = inputs?.isAftDistanceToFwd ?? true,


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
                MeanAdjustedDraught = results?.MeanAdjustedDraught,
                MeanAdjustedDraughtAfterKeelCorrection = results?.MeanAdjustedDraughtAfterKeelCorrection  
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
                .Include(b =>b.DraughtsResults)
                .Include(b => b.HydrostaticInput)
                .Include(b => b.HydrostaticResults)
                .Include(b => b.DeductiblesInput)
                .Include(b => b.DeductiblesResults)
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

            input.DraughtFwdPS = viewModel.DraughtFwdPS;
            input.DraughtFwdSS = viewModel.DraughtFwdSS;
            input.DraughtMidPS = viewModel.DraughtMidPS;
            input.DraughtMidSS = viewModel.DraughtMidSS;
            input.DraughtAftPS = viewModel.DraughtAftPS;
            input.DraughtAftSS = viewModel.DraughtAftSS;

            input.DistanceFwd = viewModel.DistanceFwd;
            input.DistanceMid = viewModel.DistanceMid;
            input.DistanceAft = viewModel.DistanceAft;

            input.isFwdDistancetoFwd = viewModel.IsFwdDistancetoFwd;
            input.isMidDistanceToFwd = viewModel.IsMidDistanceToFwd;
            input.isAftDistanceToFwd = viewModel.IsAftDistanceToFwd;

            input.SeaWaterDensity = viewModel.SeaWaterDensity;
            input.KeelCorrection = viewModel.KeelCorrection;


            _surveyCalculationsService.RecalculateAll(draughtSurveyBlock);


            //double? fwdMean = null;
            //double? draughtFwdPS = input.DraughtFwdPS;
            //double? draughtFwdSS = input.DraughtFwdSS;

            //if (draughtFwdPS.HasValue &&
            //    draughtFwdSS.HasValue)
            //{
            //    fwdMean = _surveyCalculationsService.CalculateApparentMean(
            //        draughtFwdPS.Value,
            //        draughtFwdSS.Value
            //        );
            //}

            //double? midMean = null;
            //double? draughtMidPS = input.DraughtMidPS;
            //double? draughtMidSS = input.DraughtMidSS;

            //if (draughtMidPS.HasValue &&
            //    draughtMidSS.HasValue)
            //{
            //    midMean = _surveyCalculationsService.CalculateApparentMean(
            //        draughtMidPS.Value,
            //        draughtMidSS.Value
            //        );
            //}

            //double? aftMean = null;
            //double? draughtAftPS = input.DraughtAftPS;
            //double? draughtAftSS = input.DraughtAftSS;

            //if (draughtAftPS.HasValue &&
            //    draughtAftSS.HasValue)
            //{
            //    aftMean = _surveyCalculationsService.CalculateApparentMean(
            //        draughtAftPS.Value,
            //        draughtAftSS.Value
            //        );
            //}

            //double? trimApparent = null;

            //if (fwdMean.HasValue &&
            //    aftMean.HasValue)
            //{
            //    trimApparent = _surveyCalculationsService.CalculateTrim(
            //        fwdMean.Value,
            //        aftMean.Value
            //        );
            //}




            //double? bm = draughtSurveyBlock.Inspection?.VesselInput?.BM;
            //double? lbp = draughtSurveyBlock.Inspection?.VesselInput?.LBP;

            //double? lbd = null;
            //double? distanceFwd = input.DistanceFwd;
            //bool? isFwdDistancetoFwd = input.isFwdDistancetoFwd;
            //double? distanceAft = input.DistanceAft;
            //bool? isAftDistanceToFwd = input.isAftDistanceToFwd;

            //if (lbp.HasValue &&
            //    distanceFwd.HasValue &&
            //    isFwdDistancetoFwd.HasValue &&
            //    distanceAft.HasValue &&
            //    isAftDistanceToFwd.HasValue
            //    )
            //{
            //    lbd = _surveyCalculationsService.CalculatLBD(
            //        lbp.Value,
            //        distanceFwd.Value,
            //        isFwdDistancetoFwd.Value,
            //        distanceAft.Value,
            //        isAftDistanceToFwd.Value
            //        );
            //}


            //double? draughtCorrectionFwd = null;

            //if (distanceFwd.HasValue && 
            //    trimApparent.HasValue && 
            //    isFwdDistancetoFwd.HasValue && 
            //    lbd.HasValue)
            //{
            //    draughtCorrectionFwd = _surveyCalculationsService.CalculateTrimCorrection(
            //        distanceFwd.Value,
            //        trimApparent.Value,
            //        isFwdDistancetoFwd.Value,
            //        lbd.Value
            //        );
            //}

            //double? draughtCorrectionMid = null;

            //double? distanceMid = input.DistanceMid;
            //bool? isMidDistanceToFwd = input.isMidDistanceToFwd;                

            //if (distanceMid.HasValue && 
            //    trimApparent.HasValue && 
            //    isMidDistanceToFwd.HasValue &&
            //    lbd.HasValue)
            //{
            //    draughtCorrectionMid = _surveyCalculationsService.CalculateTrimCorrection(
            //        distanceMid.Value,                    
            //        trimApparent.Value,
            //        isMidDistanceToFwd.Value,
            //        lbd.Value
            //        );
            //}


            //double? draughtCorrectionAft = null;

            //if (distanceAft.HasValue &&
            //    trimApparent.HasValue &&
            //    isAftDistanceToFwd.HasValue &&
            //    lbd.HasValue)
            //{
            //    draughtCorrectionAft = _surveyCalculationsService.CalculateTrimCorrection(
            //        distanceAft.Value,
            //        trimApparent.Value,
            //        isAftDistanceToFwd.Value,
            //        lbd.Value
            //        );
            //}


            //double? draughtCorrectedFwd = null;

            //if (fwdMean.HasValue &&
            //    draughtCorrectionFwd.HasValue)
            //{
            //    draughtCorrectedFwd = _surveyCalculationsService.CalculateCorrectedDraught(
            //    fwdMean.Value,
            //    draughtCorrectionFwd.Value
            //    );
            //}



            //double? draughtCorrectedMid = null;

            //if (midMean.HasValue &&
            //    draughtCorrectionMid.HasValue)
            //{
            //    draughtCorrectedMid = _surveyCalculationsService.CalculateCorrectedDraught(
            //        midMean.Value,
            //        draughtCorrectionMid.Value
            //        );
            //}


            //double? draughtCorrectedAft = null;

            //if (aftMean.HasValue &&
            //    draughtCorrectionAft.HasValue)
            //{
            //    draughtCorrectedAft = _surveyCalculationsService.CalculateCorrectedDraught(
            //        aftMean.Value,
            //        draughtCorrectionAft.Value
            //        );
            //}


            //double? trimCorrected = null;

            //if (draughtCorrectedFwd.HasValue && 
            //    draughtCorrectedAft.HasValue)
            //{
            //    trimCorrected = _surveyCalculationsService.CalculateTrim(
            //        draughtCorrectedFwd.Value,
            //        draughtCorrectedAft.Value
            //        );
            //}


            //double? heel = null;

            //if (draughtMidPS.HasValue &&
            //    draughtMidSS.HasValue &&
            //    bm.HasValue)
            //{
            //    heel = _surveyCalculationsService.CalculateHeel(
            //        draughtMidPS.Value,
            //        draughtMidSS.Value,
            //        bm.Value
            //        );
            //}


            //double? hogSag = null;

            //if (draughtCorrectedFwd.HasValue && 
            //    draughtCorrectedMid.HasValue && 
            //    draughtCorrectedAft.HasValue)
            //{
            //    hogSag = _surveyCalculationsService.CalculateHoggingSagging(
            //        draughtCorrectedFwd.Value,
            //        draughtCorrectedMid.Value,
            //        draughtCorrectedAft.Value);
            //}


            //double? meanAdjustedDraught = null;

            //if (draughtCorrectedFwd.HasValue &&
            //    draughtCorrectedMid.HasValue &&
            //    draughtCorrectedAft.HasValue)
            //{
            //    meanAdjustedDraught = _surveyCalculationsService.CalculateMeanOfMean(
            //        draughtCorrectedFwd.Value,
            //        draughtCorrectedMid.Value,
            //        draughtCorrectedAft.Value);
            //}


            //double? meanAdjustedDraughtAfterKeelCorrection = null;
            //double? keelCorrection = input.KeelCorrection;

            //if (!keelCorrection.HasValue)
            //{
            //    keelCorrection = 0;
            //}

            //if (meanAdjustedDraught.HasValue &&
            //    keelCorrection.HasValue)
            //{
            //    meanAdjustedDraughtAfterKeelCorrection = _surveyCalculationsService.CalculateMeanAdjustedDraughtAfterKeelCorrection(
            //        meanAdjustedDraught.Value,
            //        keelCorrection.Value
            //        );
            //}          




            //double hoggingSagging = _surveyCalculationsService.CalculateHoggingSagging()

            //double fwdCorrection = _surveyCalculationsService.CalculateTrimCorrection


            //var results = await _context.DraughtsResults
            //    .FirstOrDefaultAsync(r => r.DraughtSurveyBlockId == draughtSurveyBlock.Id);

            //if (results == null)
            //{
            //    results = new Models.DraughtsResults
            //    {
            //        DraughtSurveyBlockId = draughtSurveyBlock.Id,
            //        DraughtSurveyBlock = draughtSurveyBlock
            //    };

            //    _context.DraughtsResults.Add(results);
            //}

            //results.DraughtMeanFwd = fwdMean;
            //results.DraughtMeanMid = midMean;
            //results.DraughtMeanAft = aftMean;

            //results.TrimApparent = trimApparent;

            //results.DraughtCorrectionFwd = draughtCorrectionFwd;
            //results.DraughtCorrectionMid = draughtCorrectionMid;
            //results.DraughtCorrectionAft = draughtCorrectionAft;

            //results.DraughtCorrectedFwd = draughtCorrectedFwd;
            //results.DraughtCorrectedMid = draughtCorrectedMid;
            //results.DraughtCorrectedAft = draughtCorrectedAft;

            //results.TrimCorrected = trimCorrected;
            //results.Heel = heel;
            //results.HoggingSagging = hogSag;
            //results.MeanAdjustedDraught = meanAdjustedDraught;
            //results.MeanAdjustedDraughtAfterKeelCorrection = meanAdjustedDraughtAfterKeelCorrection;

            await _context.SaveChangesAsync();

            //return View(viewModel);
            //return RedirectToAction("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId });
            return Redirect($"{Url.Action("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId })}#initial-draught-draughts");
        }


        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
