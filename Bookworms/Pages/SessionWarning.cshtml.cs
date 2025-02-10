using Bookworms.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Bookworms.Pages
{


    public class SessionWarningModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SessionWarningModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostContinue()
        {
            // Allow the current session to continue
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostLogoutOthers()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                // Generate a new session ID (Invalidate other sessions)
                user.CurrentSessionId = System.Guid.NewGuid().ToString();
                await _userManager.UpdateAsync(user);

                // Sign out the current session and re-authenticate with the new session ID
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, isPersistent: true);

                return RedirectToPage("Index");
            }
            return RedirectToPage("Login");
        }
    }


}
