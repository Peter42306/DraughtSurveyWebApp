using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.Services;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DraughtSurveyWebApp.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class DraughtsInputController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SurveyCalculationsService _surveyCalculationsService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DraughtsInputController> _logger;

        public DraughtsInputController(
            ApplicationDbContext context, 
            SurveyCalculationsService surveyCalculationsService,
            UserManager<ApplicationUser> userManager,
            ILogger<DraughtsInputController> logger)
        {
            _context = context;
            _surveyCalculationsService = surveyCalculationsService;
            _userManager = userManager;
            _logger=logger;
        }

        // GET: DraughtsInput/Edit?draughtSurveyBlockId=5
        public async Task<IActionResult> Edit(int draughtSurveyBlockId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }


            var draughtSurveyBlock = await _context.DraughtSurveyBlocks
                .Include(b => b.DraughtsInput)
                .Include(b => b.DraughtsResults)
                .Include(b => b.HydrostaticInput)
                .Include(b => b.HydrostaticResults)
                .Include(b => b.DeductiblesInput)
                .Include(b => b.DeductiblesResults)
                .Include(b => b.Inspection)
                    .ThenInclude(i => i.VesselInput)
                .FirstOrDefaultAsync(b => b.Id == draughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }

            if (draughtSurveyBlock.Inspection == null)
            {
                return NotFound();
            }           
            

            

            if (draughtSurveyBlock.Inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            
            var inputs = draughtSurveyBlock.DraughtsInput;
            var results = draughtSurveyBlock.DraughtsResults;

            //var test = draughtSurveyBlock.SurveyType.ToString();

            var viewModel = new DraughtsInputViewModel
            {
                DraughtSurveyBlockId = draughtSurveyBlock.Id,
                InspectionId = draughtSurveyBlock.InspectionId,

                SurveyType = draughtSurveyBlock.SurveyType,

                DraughtFwdPS = inputs?.DraughtFwdPS,
                DraughtFwdSS = inputs?.DraughtFwdSS,
                DraughtMidPS = inputs?.DraughtMidPS,
                DraughtMidSS = inputs?.DraughtMidSS,
                DraughtAftPS = inputs?.DraughtAftPS,
                DraughtAftSS = inputs?.DraughtAftSS,

                DistanceFwd = inputs?.DistanceFwd,
                DistanceMid = inputs?.DistanceMid,
                DistanceAft = inputs?.DistanceAft,

                BreadthForward = inputs?.BreadthForward,
                BreadthAft = inputs?.BreadthAft,

                KeelCorrection = inputs?.KeelCorrection,
                SeaWaterDensity = inputs?.SeaWaterDensity,
                Swell = inputs?.Swell,

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
                LBD = results?.LBD,
                MeanAdjustedDraught = results?.MeanAdjustedDraught,
                MeanAdjustedDraughtAfterKeelCorrection = results?.MeanAdjustedDraughtAfterKeelCorrection  
            };

            // Check if distances are not filled in the inputs
            bool distancesNotFilled =
                inputs == null ||
                inputs.DistanceFwd == null ||
                inputs.DistanceMid == null ||
                inputs.DistanceAft == null;
            

            if (distancesNotFilled && draughtSurveyBlock.Inspection?.VesselInput?.IMO != null)
            {                

                string imo = draughtSurveyBlock.Inspection.VesselInput.IMO;

                var candidatesForDistances = await _context.DraughtsInputs
                    .Include(d => d.DraughtSurveyBlock)
                        .ThenInclude(r => r.DraughtsResults)
                    .Include(d => d.DraughtSurveyBlock.Inspection)
                            .ThenInclude(i => i.VesselInput)
                    .Where(d =>
                        d.DraughtSurveyBlock != null &&
                        d.DraughtSurveyBlock.Inspection != null &&
                        d.DraughtSurveyBlock.Inspection.VesselInput != null &&
                        d.DraughtSurveyBlock.DraughtsResults != null &&
                        d.DraughtSurveyBlock.Inspection.ApplicationUserId == user.Id &&
                        d.DraughtSurveyBlock.DraughtsResults.MeanAdjustedDraughtAfterKeelCorrection != null &&
                        d.DistanceFwd !=null && d.DistanceMid != null && d.DistanceAft != null &&
                        d.isFwdDistancetoFwd != null && d.isMidDistanceToFwd !=null && d.isAftDistanceToFwd != null &&
                        d.DraughtSurveyBlock.Inspection.VesselInput.IMO == imo)
                    .ToListAsync();

                

                var filteredCandidatesForDistances = candidatesForDistances
                    .GroupBy(d => new
                    {
                        d.DistanceFwd,
                        d.DistanceMid,
                        d.DistanceAft,
                        d.isFwdDistancetoFwd,
                        d.isMidDistanceToFwd,
                        d.isAftDistanceToFwd
                    })
                    .Select(g => g.First())
                    .ToList();


                

                if (filteredCandidatesForDistances.Any())                {
                    

                    string distanceShiftedTo(double? distance, bool? isShiftedToFwd)
                    {
                        if (distance == 0 || distance == null || isShiftedToFwd == null)
                        {
                            return "";
                        }

                        return isShiftedToFwd == true ? $"(to Fwd)" : "(to Aft)";
                    }

                    var messages = new List<string>();

                    foreach (var item in filteredCandidatesForDistances)
                    {
                        string draught = item?.DraughtSurveyBlock?.DraughtsResults?.MeanAdjustedDraughtAfterKeelCorrection?.ToString("N3") ?? "";

                        var message = $"For draught: {draught} m, <br/>" +
                            $"Distance Fwd: {item?.DistanceFwd?.ToString("N3")} m {distanceShiftedTo(item?.DistanceFwd, item?.isFwdDistancetoFwd)}, <br/>" +
                            $"Distance Mid: {item?.DistanceMid?.ToString("N3")} m {distanceShiftedTo(item?.DistanceMid, item?.isMidDistanceToFwd)}, <br/>" +
                            $"Distance Aft: {item?.DistanceAft?.ToString("N3")} m {distanceShiftedTo(item?.DistanceAft, item?.isAftDistanceToFwd)}.";

                        messages.Add(message);
                    }



                    ViewData["DistancesSuggestionNote"] = $"Distances based on your previous inspections: <hr/>" +
                        string.Join("<hr/>", messages);                    
                }

            }            
            
            viewModel.Inspection = draughtSurveyBlock.Inspection;
            viewModel.VesselInput = draughtSurveyBlock?.Inspection?.VesselInput;
            return View(viewModel);
        }

        // POST: DraughtsInput/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DraughtsInputViewModel viewModel)
        {
            


            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }


            var draughtSurveyBlock = await _context.DraughtSurveyBlocks
                .Include(b => b.DraughtsInput)
                .Include(b => b.DraughtsResults)
                .Include(b => b.HydrostaticInput)
                .Include(b => b.HydrostaticResults)
                .Include(b => b.DeductiblesInput)
                .Include(b => b.DeductiblesResults)
                .Include(b => b.Inspection)
                    .ThenInclude(i => i.VesselInput)
                .Include(b => b.Inspection)
                    .ThenInclude(i => i.CargoResult)
                .Include(b => b.Inspection)
                    .ThenInclude(i => i.CargoInput)
                .Include(b => b.Inspection)
                    .ThenInclude(i => i.DraughtSurveyBlocks)
                .FirstOrDefaultAsync(b => b.Id == viewModel.DraughtSurveyBlockId);

            if (draughtSurveyBlock == null)
            {
                return NotFound();
            }            

            if (draughtSurveyBlock.Inspection.ApplicationUserId != user.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }           
                        

            if (draughtSurveyBlock.DraughtsInput == null)
            {
                draughtSurveyBlock.DraughtsInput = new Models.DraughtsInput
                {
                    DraughtSurveyBlockId = viewModel.DraughtSurveyBlockId,
                    DraughtSurveyBlock = draughtSurveyBlock
                };

                _context.DraughtsInputs.Add(draughtSurveyBlock.DraughtsInput);
            }

            if (draughtSurveyBlock.DraughtsResults == null)
            {
                draughtSurveyBlock.DraughtsResults = new Models.DraughtsResults
                {
                    DraughtSurveyBlockId = draughtSurveyBlock.Id,
                    DraughtSurveyBlock = draughtSurveyBlock
                };

                _context.DraughtsResults.Add(draughtSurveyBlock.DraughtsResults);
            }

            var input = draughtSurveyBlock.DraughtsInput;

            bool changed = IsDraughtsInputChanged(input, viewModel);

            if (changed)
            {
                input.DraughtFwdPS = viewModel.DraughtFwdPS;
                input.DraughtFwdSS = viewModel.DraughtFwdSS;
                input.DraughtMidPS = viewModel.DraughtMidPS;
                input.DraughtMidSS = viewModel.DraughtMidSS;
                input.DraughtAftPS = viewModel.DraughtAftPS;
                input.DraughtAftSS = viewModel.DraughtAftSS;

                input.DistanceFwd = viewModel.DistanceFwd;
                input.DistanceMid = viewModel.DistanceMid;
                input.DistanceAft = viewModel.DistanceAft;

                input.BreadthForward = viewModel.BreadthForward;
                input.BreadthAft = viewModel.BreadthAft;

                input.isFwdDistancetoFwd = viewModel.IsFwdDistancetoFwd;
                input.isMidDistanceToFwd = viewModel.IsMidDistanceToFwd;
                input.isAftDistanceToFwd = viewModel.IsAftDistanceToFwd;

                input.SeaWaterDensity = viewModel.SeaWaterDensity;
                input.KeelCorrection = viewModel.KeelCorrection;
                input.Swell = viewModel.Swell;

                
                await _context.SaveChangesAsync();
                _surveyCalculationsService.RecalculateAll(draughtSurveyBlock);
            }

            _surveyCalculationsService.RecalculateAll(draughtSurveyBlock);


            // Recalculation of results if available 

            var initialBlock = await _context.DraughtSurveyBlocks
                .Include(b => b.HydrostaticResults)
                .FirstOrDefaultAsync(b =>
                b.InspectionId == draughtSurveyBlock.InspectionId &&
                b.SurveyType == SurveyType.Initial);

            var finalBlock = await _context.DraughtSurveyBlocks
                .Include(b => b.HydrostaticResults)
                .FirstOrDefaultAsync(b =>
                b.InspectionId == draughtSurveyBlock.InspectionId &&
                b.SurveyType == SurveyType.Final);

            var initialNetto = initialBlock?.HydrostaticResults?.NettoDisplacement;
            var finalNetto = finalBlock?.HydrostaticResults?.NettoDisplacement;


            double? cargoByDraughtSurvey = null;
            double? differenceWithBL_Mt = null;
            double? differenceWithBL_Percents = null;
            double? differenceWithSDWT_Percents = null;

            // Calculate cargo by draught survey
            if (initialNetto.HasValue && finalNetto.HasValue)
            {
                cargoByDraughtSurvey = Math.Abs(Math.Round(finalNetto.Value - initialNetto.Value, 3, MidpointRounding.AwayFromZero));
            }

            if (draughtSurveyBlock.Inspection.CargoResult == null)
            {
                draughtSurveyBlock.Inspection.CargoResult = new CargoResult
                {
                    InspectionId = draughtSurveyBlock.InspectionId,
                    Inspection = draughtSurveyBlock.Inspection
                };
            }


            // Calculate difference with Bill of Lading
            var declaredWeight = draughtSurveyBlock.Inspection.CargoInput?.DeclaredWeight;
            _logger.LogWarning($"var declaredWeight = draughtSurveyBlock.Inspection.CargoInput?.DeclaredWeight; {declaredWeight}");

            if (cargoByDraughtSurvey.HasValue && declaredWeight.HasValue)
            {
                differenceWithBL_Mt = Math.Round(cargoByDraughtSurvey.Value - declaredWeight.Value, 3, MidpointRounding.AwayFromZero);
                differenceWithBL_Percents = Math.Round((differenceWithBL_Mt.Value / declaredWeight.Value) * 100, 3, MidpointRounding.AwayFromZero);
            }

            // Calculate difference with SDWT
            var sdwt = draughtSurveyBlock.Inspection.VesselInput?.SDWT;
            _logger.LogWarning($"var sdwt = draughtSurveyBlock.Inspection.VesselInput?.SDWT; {sdwt}");

            if (differenceWithBL_Mt.HasValue && sdwt.HasValue)
            {
                differenceWithSDWT_Percents = Math.Round((differenceWithBL_Mt.Value / sdwt.Value) * 100, 3, MidpointRounding.AwayFromZero);
            }


            // Update CargoResult with calculated values
            draughtSurveyBlock.Inspection.CargoResult.CargoByDraughtSurvey = cargoByDraughtSurvey;
            draughtSurveyBlock.Inspection.CargoResult.DifferenceWithBL_Mt = differenceWithBL_Mt;
            draughtSurveyBlock.Inspection.CargoResult.DifferenceWithBL_Percents = differenceWithBL_Percents;
            draughtSurveyBlock.Inspection.CargoResult.DifferenceWithSDWT_Percents = differenceWithSDWT_Percents;



            await _context.SaveChangesAsync();

            string anchor = draughtSurveyBlock.SurveyType == SurveyType.Initial 
                ? "initial-draught-draughts" 
                : "final-draught-draughts";

            return Redirect($"{Url.Action("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId })}#{anchor}");
        }

        private bool IsDraughtsInputChanged(DraughtsInput dbValue, DraughtsInputViewModel viewModelValue)
        {
            return
                dbValue.DraughtFwdPS != viewModelValue.DraughtFwdPS ||
                dbValue.DraughtFwdSS != viewModelValue.DraughtFwdSS ||
                dbValue.DraughtMidPS != viewModelValue.DraughtMidPS ||
                dbValue.DraughtMidSS != viewModelValue.DraughtMidSS ||
                dbValue.DraughtAftPS != viewModelValue.DraughtAftPS ||
                dbValue.DraughtAftSS != viewModelValue.DraughtAftSS ||

                dbValue.DistanceFwd != viewModelValue.DistanceFwd ||
                dbValue.DistanceMid != viewModelValue.DistanceMid ||
                dbValue.DistanceAft != viewModelValue.DistanceAft ||

                dbValue.BreadthForward != viewModelValue.BreadthForward ||
                dbValue.BreadthAft != viewModelValue.BreadthAft ||

                dbValue.isFwdDistancetoFwd != viewModelValue.IsFwdDistancetoFwd ||
                dbValue.isMidDistanceToFwd!= viewModelValue.IsMidDistanceToFwd ||
                dbValue.isAftDistanceToFwd != viewModelValue.IsAftDistanceToFwd ||

                dbValue.SeaWaterDensity != viewModelValue.SeaWaterDensity ||
                dbValue.KeelCorrection != viewModelValue.KeelCorrection ||
                dbValue.Swell != viewModelValue.Swell;
        }        
    }
}
