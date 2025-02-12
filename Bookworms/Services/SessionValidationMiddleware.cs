using Bookworms.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;

    public SessionValidationMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _scopeFactory = scopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Allow /SessionWarning to load without redirection loop
        if (context.Request.Path.StartsWithSegments("/SessionWarning"))
        {
            await _next(context);
            return;
        }

        if (context.User.Identity.IsAuthenticated)
        {
            var sessionClaim = context.User.FindFirst("CurrentSessionId")?.Value;
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(sessionClaim))
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var user = await userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        if (user.CurrentSessionId != sessionClaim)
                        {
                            Console.WriteLine("Session mismatch detected. Logging out expired session.");
							var returnUrl = "/Login?sessionExpired=true";


                            // Sign out the invalid session
                            await context.SignOutAsync(IdentityConstants.ApplicationScheme);

                            // Redirect to login page
                            context.Response.Redirect(returnUrl);
                            return;
                        }
                    }
                }
            }

            var lastActivity = context.Session.GetString("LastActivityTime");
            if (!string.IsNullOrEmpty(lastActivity))
            {
                var lastActivityTime = DateTime.Parse(lastActivity);
                if ((DateTime.UtcNow - lastActivityTime).TotalMinutes > 5) 
                {
                    context.Session.Clear(); 
                    await context.SignOutAsync(IdentityConstants.ApplicationScheme);
					context.Response.Redirect($"/Login?timeout=true&ReturnUrl={Uri.EscapeDataString(context.Request.Path)}");
					Console.WriteLine("Redirecting due to session timeout: " + $"/Login?timeout=true&ReturnUrl={context.Request.Path}");

					return;
                }
            }

            // Update session activity time
            context.Session.SetString("LastActivityTime", DateTime.UtcNow.ToString());

        }

        await _next(context);
    }
}
