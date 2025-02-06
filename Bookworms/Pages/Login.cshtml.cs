using Bookworms.Models;
using Bookworms.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
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

		private readonly SignInManager<ApplicationUser> signInManager;
		public LoginModel(SignInManager<ApplicationUser> signInManager)
		{
			this.signInManager = signInManager;
		}

		public void OnGet()
        {
        }

		// Helper method to verify reCAPTCHA v3 token
		private async Task<bool> VerifyRecaptchaAsync(string token)
		{
			// Replace with your actual secret key
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
				if (identityResult.Succeeded)
				{
					//Create the security context
					var claims = new List<Claim> {
					new Claim(ClaimTypes.Name, "c@c.com"),
					new Claim(ClaimTypes.Email, "c@c.com"),

                    new Claim("Department", "HR")
                    };
					var i = new ClaimsIdentity(claims, "MyCookieAuth");
					ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Or your desired expiry time
                    };
                    await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authProperties);

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
