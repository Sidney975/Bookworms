using Bookworms.Models;
using Bookworms.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookworms.Pages
{
    public class LogoutModel : PageModel
    {
		private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AuditLogService _auditLogService;
        private readonly UserManager<ApplicationUser> _userManager;
        public LogoutModel(SignInManager<ApplicationUser> signInManager, AuditLogService auditLogService, UserManager<ApplicationUser> userManager)
        {
            this.signInManager = signInManager;
            _auditLogService = auditLogService;
            _userManager = userManager;
        }
        public void OnGet() 
		{ 
		}
		
		public async Task<IActionResult> OnPostLogoutAsync()
		{
            await signInManager.SignOutAsync();
			return RedirectToPage("Login");
		}
		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Index");
		}
	}
}
