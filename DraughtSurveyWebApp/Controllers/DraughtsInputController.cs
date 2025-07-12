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
                        string draught = item.DraughtSurveyBlock.DraughtsResults.MeanAdjustedDraughtAfterKeelCorrection?.ToString("N3") ?? "";

                        var message = $"For draught: {draught} m, <br/>" +
                            $"Distance Fwd: {item.DistanceFwd?.ToString("N3")} m {distanceShiftedTo(item.DistanceFwd, item.isFwdDistancetoFwd)}, <br/>" +
                            $"Distance Mid: {item.DistanceMid?.ToString("N3")} m {distanceShiftedTo(item.DistanceMid, item.isMidDistanceToFwd)}, <br/>" +
                            $"Distance Aft: {item.DistanceAft?.ToString("N3")} m {distanceShiftedTo(item.DistanceAft, item.isAftDistanceToFwd)}.";

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

                input.isFwdDistancetoFwd = viewModel.IsFwdDistancetoFwd;
                input.isMidDistanceToFwd = viewModel.IsMidDistanceToFwd;
                input.isAftDistanceToFwd = viewModel.IsAftDistanceToFwd;

                input.SeaWaterDensity = viewModel.SeaWaterDensity;
                input.KeelCorrection = viewModel.KeelCorrection;
            }

            _surveyCalculationsService.RecalculateAll(draughtSurveyBlock);

            await _context.SaveChangesAsync();

            //return View(viewModel);
            //return RedirectToAction("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId });
            return Redirect($"{Url.Action("Details", "Inspections", new { id = draughtSurveyBlock.InspectionId })}#initial-draught-draughts");
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

                dbValue.isFwdDistancetoFwd != viewModelValue.IsFwdDistancetoFwd ||
                dbValue.isMidDistanceToFwd!= viewModelValue.IsMidDistanceToFwd ||
                dbValue.isAftDistanceToFwd != viewModelValue.IsAftDistanceToFwd ||

                dbValue.SeaWaterDensity != viewModelValue.SeaWaterDensity ||
                dbValue.KeelCorrection != viewModelValue.KeelCorrection;
        }        
    }
}
