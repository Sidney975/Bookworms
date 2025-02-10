using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Bookworms.Services
{
	public class OutlookEmailSender : IEmailSender
	{
		private readonly IConfiguration _configuration;

		public OutlookEmailSender(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			// Get SMTP settings from configuration
			var smtpHost = _configuration["EmailSettings:SmtpHost"];
			var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
			var smtpUser = _configuration["EmailSettings:SmtpUser"];
			var smtpPass = _configuration["EmailSettings:SmtpPass"];
			var fromEmail = _configuration["EmailSettings:FromEmail"];

			using (var message = new MailMessage())
			{
				message.From = new MailAddress(fromEmail);
				message.To.Add(new MailAddress(email));
				message.Subject = subject;
				message.Body = htmlMessage;
				message.IsBodyHtml = true;

				using (var client = new SmtpClient(smtpHost, smtpPort))
				{
					client.Credentials = new NetworkCredential(smtpUser, smtpPass);
					client.EnableSsl = true;
					await client.SendMailAsync(message);
				}
			}
		}
	}
}
