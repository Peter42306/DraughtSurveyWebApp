using DraughtSurveyWebApp.Data;
using DraughtSurveyWebApp.Models;
using DraughtSurveyWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DraughtSurveyWebApp.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ContactController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new FeedbackTicketViewModel());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendFeedback(FeedbackTicketViewModel viewModel)
        {
            var msg = viewModel.Message.Trim();

            if (string.IsNullOrWhiteSpace(msg) || msg.Length < 5)
            {
                ModelState.AddModelError(nameof(viewModel.Message), "Please enter at least 5 characters");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var email = await _userManager.GetEmailAsync(user) ?? "(no email)";

            var request = new FeedbackRequestDto
            {
                AppKey = _configuration["ContactFormApi:AppKey"]!,
                UserId = user.Id,
                SenderEmail = email,
                Type = 1,
                Subject = "Draught Survey Web App feedback",
                Body = msg
            };

            var client = _httpClientFactory.CreateClient("ContactFormApi");

            try
            {
                var response = await client.PostAsJsonAsync("api/feedback", request);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Your message could not be sent. Please try again.");

                    return View(nameof(Index), viewModel);
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The feedback service is currently unavailable. Please try again later.");

                return View(nameof(Index), viewModel);
            }
                       

            var ticket = new FeedbackTicket
            {
                ApplicationUserId = user.Id,
                ApplicationUser = user,
                UserEmail = email,
                Message = msg
            };

            _context.FeedbackTickets.Add(ticket);
            await _context.SaveChangesAsync();

            TempData["FeedbackSent"] = "Thank you! Your message has been sent.";
            return RedirectToAction(nameof(Index));
        }
    }
}
