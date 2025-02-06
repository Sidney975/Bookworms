using Bookworms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookworms.Pages
{
    [Authorize]
    public class HomeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public string DecryptedCreditCard { get; set; }


        public ApplicationUser UserProfile { get; set; }

        public HomeModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
            var protector = dataProtectionProvider.CreateProtector("MySecretKey");

            UserProfile = await _userManager.GetUserAsync(User);
            if (UserProfile != null)
            {
                // Decrypt the credit card field
                DecryptedCreditCard = protector.Unprotect(UserProfile.CreditCard);
            }
        }
    }

}
