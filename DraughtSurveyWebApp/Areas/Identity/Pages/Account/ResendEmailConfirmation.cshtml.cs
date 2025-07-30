using DraughtSurveyWebApp.Interfaces;
using DraughtSurveyWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace DraughtSurveyWebApp.Areas.Identity.Pages.Account
{
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ResendEmailConfirmationModel> _logger;

        public ResendEmailConfirmationModel(
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            ILogger<ResendEmailConfirmationModel> logger)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnGet()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[ResendConfirmation] Invalid model for email: {Email}", Input.Email);
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                _logger.LogWarning("[ResendConfirmation] Email not found: {Email}", Input.Email);
                return RedirectToPage("./ResendEmailConfirmationConfirmation");
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogWarning("[ResendConfirmation] Email already confirmed: {Email}", Input.Email);
                return RedirectToPage("./ResendEmailConfirmationConfirmation");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodeToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var confirmationLink = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler:null,
                values: new { userId = user.Id, code = encodeToken },
                protocol: Request.Scheme);

            _logger.LogWarning("[ResendConfirmation] Sending email to: {Email}", user.Email);
            _logger.LogWarning("[ResendConfirmation] Confirmation link: {Link}", confirmationLink);


            try
            {
                if (user.Email != null && confirmationLink != null)
                {
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicking here</a>.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ResendConfirmation] Failed to send email to: {Email}", user.Email);
            }
                       

            return RedirectToPage("./ResendEmailConfirmationConfirmation");
        }
    }
}
