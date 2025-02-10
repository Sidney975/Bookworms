using Bookworms.Models;
using Microsoft.AspNetCore.Identity;

namespace Bookworms.Services
{
	public class PasswordAgeMiddleware
	{
		private readonly RequestDelegate _next;

		public PasswordAgeMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			if (context.User.Identity.IsAuthenticated)
			{
				var user = await userManager.GetUserAsync(context.User);
				if (user != null)
				{
					// Define maximum password age (e.g., 90 days)
					TimeSpan maxPasswordAge = TimeSpan.FromDays(90);

					if (DateTime.UtcNow - user.LastPasswordChangeDate > maxPasswordAge)
					{
						// Sign out user and force password change
						await signInManager.SignOutAsync();
						context.Response.Redirect("/ChangePassword");
						return;
					}
				}
			}

			await _next(context);
		}
	}

}
