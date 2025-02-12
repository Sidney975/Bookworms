using Bookworms.Models;
using Bookworms.Services;
using Bookworms.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;

namespace Bookworms.Pages
{
    public class LoginModel : PageModel
    {
		[BindProperty]
		public Login LModel { get; set; }

		[BindProperty]
		public bool AnotherSession { get; set; }

        [BindProperty]
		public bool SessionExpired { get; set; }

		private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly AuditLogService _auditLogService;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender,
            AuditLogService auditLogService)
		{
			this.signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _auditLogService = auditLogService;

        }

        public void OnGet()
        {
			if (Request.Query.ContainsKey("sessionExpired"))
			{
				AnotherSession = true;
			}
            if (Request.Query.ContainsKey("timeout"))
            {
                SessionExpired = true;
            }
            
        }

		// Helper method to verify reCAPTCHA v3 token
		private async Task<bool> VerifyRecaptchaAsync(string token)
		{
			var secretKey = "6LdSwc4qAAAAAG_HQSHRM1FG6ru90-SOP57RVNty";
			using (var client = new HttpClient())
			{
				var values = new Dictionary<string, string>
				{
					{ "secret", secretKey },
					{ "response", token }
				};

				var content = new FormUrlEncodedContent(values);
				var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
				var jsonResponse = await response.Content.ReadAsStringAsync();

				// Deserialize the JSON response from Google
				var recaptchaResult = JsonSerializer.Deserialize<RecaptchaResponse>(jsonResponse);

				// Check that the response is successful, the action is "Login", and score is above your threshold (e.g., 0.5)
				return recaptchaResult != null && recaptchaResult.success
					   && recaptchaResult.action == "Login"
					   && recaptchaResult.score >= 0.5f;
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{

				var recaptchaToken = Request.Form["g-recaptcha-response"];
				if (string.IsNullOrWhiteSpace(recaptchaToken) || !await VerifyRecaptchaAsync(recaptchaToken))
				{
					ModelState.AddModelError("", "Captcha verification failed. Please try again.");
					return Page();
				}

                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email,
				LModel.Password, LModel.RememberMe, lockoutOnFailure: true);
                Console.WriteLine($"RequiresTwoFactor: {identityResult.RequiresTwoFactor}");

                if (
                    identityResult.RequiresTwoFactor
                    )
                {
                    // Get the user so we can generate a 2FA token using the "Email" provider.
                    var user = await _userManager.FindByEmailAsync(LModel.Email);
                    if (user != null)
                    {
                        // Generate the two-factor authentication token using the Email provider.
                        var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                        // Send the token to the user's email.
                        var emailMessage = $"Your two-factor authentication code is: <strong>{token}</strong>";
                        await _emailSender.SendEmailAsync(user.Email, "Your Two-Factor Authentication Code", emailMessage);

                        // Redirect to the 2FA verification page.
                        return RedirectToPage("VerifyTwoFactorCode", new { rememberMe = LModel.RememberMe });
                    }
                }
                else if (identityResult.Succeeded)
				{
                    var user = await _userManager.FindByEmailAsync(LModel.Email);
                    var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
                    Console.WriteLine($"Available 2FA providers: {string.Join(", ", providers)}");
                    if (user != null)
                    {
                        var newSessionId = Guid.NewGuid().ToString();
                        user.CurrentSessionId = newSessionId;
                        Console.WriteLine(newSessionId);
                        Console.WriteLine(user.CurrentSessionId);
                        await _auditLogService.LogAsync(user.Id, "Login", "User logged in successfully.");
                        await _userManager.UpdateAsync(user);

                        //var claims = new List<Claim> {
                        //    new Claim(ClaimTypes.NameIdentifier, user.Id),
                        //    new Claim(ClaimTypes.Name, user.Email), 

                        //    new Claim(ClaimTypes.Email, user.Email),


                        //new Claim("Department", "HR"),
                        //new Claim("CurrentSessionId", newSessionId)
                        //};

                        //Console.WriteLine($"session: {claims[4]}");


                        //var i = new ClaimsIdentity(claims, "MyCookieAuth");
                        //ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
                        //var authProperties = new AuthenticationProperties
                        //{
                        //    IsPersistent = true,
                        //    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Or your desired expiry time
                        //};
                        ////await signInManager.SignOutAsync();

                        //await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authProperties);

                        // Sign the user out first to avoid stale claims
                        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

                        // Sign the user in using ASP.NET Identity (which now uses CustomClaimsPrincipalFactory)
                        await signInManager.SignInAsync(user, isPersistent: LModel.RememberMe);

                        return RedirectToPage("Index");
                    }

                    return RedirectToPage("Index");
				}
                else if (identityResult.IsLockedOut)
                {
                    ModelState.AddModelError("", "Your account has been locked due to multiple failed login attempts. Please try again later.");
                }
                else
                {
                    ModelState.AddModelError("", "Username or Password is incorrect");
                }
			}
			return Page();
		}
	}
}
