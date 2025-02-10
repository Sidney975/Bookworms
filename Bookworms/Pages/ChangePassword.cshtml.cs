using Bookworms.Models;
using Bookworms.Services;
using Bookworms.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Bookworms.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly PasswordHistoryService _passwordHistoryService;


        public ChangePasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            PasswordHistoryService passwordHistoryService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _passwordHistoryService = passwordHistoryService;
        }

        [BindProperty]
        public ChangePassword Input { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
                //return RedirectToPage("Login");
            }

            if (await _passwordHistoryService.IsPasswordReusedAsync(user, Input.NewPassword))
            {
                ModelState.AddModelError(string.Empty, "You cannot reuse a previously used password.");
                return Page();
            }

            // Define minimum password age (e.g., 1 day)
   //         TimeSpan minPasswordAge = TimeSpan.FromDays(1);
			//if (DateTime.UtcNow - user.LastPasswordChangeDate < minPasswordAge)
			//{
			//	ModelState.AddModelError(string.Empty, $"You can change your password only once every {minPasswordAge.TotalDays} days.");
			//	return Page();
			//}

			var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.CurrentPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
            await _passwordHistoryService.StorePasswordAsync(user);

            user.LastPasswordChangeDate = DateTime.UtcNow;
			await _userManager.UpdateAsync(user);


			await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage("Index");
        }
    }
}
