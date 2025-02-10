using Bookworms.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Bookworms.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender; 

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                return RedirectToPage("ForgotPasswordConfirmation");
            }

            // Generate the password reset token.
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Build the reset link. (URL-encode the token if needed)
            var resetUrl = Url.Page(
                "/ResetPassword",
                pageHandler: null,
                values: new { email = Input.Email, token = token },
                protocol: Request.Scheme);

            // Send the email using your email service.
            await _emailSender.SendEmailAsync(Input.Email, "Reset Password",
                $"Please reset your password by <a href='{resetUrl}'>clicking here</a>.");

            return RedirectToPage("ForgotPasswordConfirmation");
        }
    }
}
