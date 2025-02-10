using Bookworms.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Bookworms.Pages
{
    public class VerifyTwoFactorCodeModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public VerifyTwoFactorCodeModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Authentication Code")]
            public string Code { get; set; }

            [Display(Name = "Remember this machine")]
            public bool RememberMe { get; set; }
        }

        // OnGet receives the RememberMe flag from the login page.
        public void OnGet(bool rememberMe)
        {
            Input = new InputModel { RememberMe = rememberMe };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToPage("Login");
            }
            // Attempt to sign in using the two-factor code with the "Email" provider.
            var result = await _signInManager.TwoFactorSignInAsync("Email", Input.Code, isPersistent: Input.RememberMe, rememberClient: false);
            if (result.Succeeded)
            {
                user.TwoFactorEnabled = false;
                await _userManager.UpdateAsync(user);
                Console.WriteLine("2FA Disabled After Successful Login");
                return RedirectToPage("Index");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account is locked out.");
                return Page();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid authentication code.");
                return Page();
            }
        }
    }
}
