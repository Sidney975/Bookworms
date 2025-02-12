using Bookworms.Models;
using Bookworms.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Bookworms.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuditLogService _auditLogService;
        private readonly PasswordHistoryService _passwordHistoryService;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager, AuditLogService auditLogService, PasswordHistoryService passwordHistoryService)
        {
            _userManager = userManager;
            _auditLogService = auditLogService;
            _passwordHistoryService = passwordHistoryService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Token { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string email, string token)
        {
            Input = new InputModel
            {
                Email = email,
                Token = token
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Do not reveal that the user does not exist.
                return RedirectToPage("ResetPasswordConfirmation");
            }

            if (await _passwordHistoryService.IsPasswordReusedAsync(user, Input.NewPassword))
            {
                ModelState.AddModelError(string.Empty, "You cannot reuse a previously used password.");
                return Page(); // Prevents token from being used
            }

            var resetResult = await _userManager.ResetPasswordAsync(user, Input.Token, Input.NewPassword);
            if (resetResult.Succeeded)
            {
                await _passwordHistoryService.StorePasswordAsync(user);

                user.LastPasswordChangeDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                await _auditLogService.LogAsync(user.Id, "Reset Password", "User has reset password.");
                // Optionally, update password history here if you have implemented that.
                return RedirectToPage("ResetPasswordConfirmation");
            }

            foreach (var error in resetResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}
