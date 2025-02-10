using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Bookworms.Services
{
	public class OutlookEmailSender : IEmailSender
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<OutlookEmailSender> _logger;


		public OutlookEmailSender(IConfiguration configuration, ILogger<OutlookEmailSender> logger)
		{
			_configuration = configuration;
			_logger = logger;
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			// Get SMTP settings from configuration
			var smtpHost = _configuration["EmailSettings:SmtpHost"];
			var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
			var smtpUser = _configuration["EmailSettings:SmtpUser"];
			var smtpPass = _configuration["EmailSettings:SmtpPass"];
			var fromEmail = _configuration["EmailSettings:FromEmail"];

			if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass) || string.IsNullOrEmpty(fromEmail))
			{
				_logger.LogError("Email configuration is missing required values.");
				throw new InvalidOperationException("Email configuration is incomplete.");
			}
			try
			{
				using (var message = new MailMessage())
				{
					message.From = new MailAddress(fromEmail);
					message.To.Add(new MailAddress(email));
					message.Subject = subject;
					if (htmlMessage.Contains("Your new password is:", StringComparison.OrdinalIgnoreCase))
					{
						_logger.LogWarning("Attempted to send an email containing sensitive data. Email blocked.");
						throw new InvalidOperationException("Sensitive data cannot be included in emails.");
					}

					message.Body = htmlMessage;
					message.IsBodyHtml = true;

					using (var client = new SmtpClient(smtpHost, smtpPort))
					{
						client.Credentials = new NetworkCredential(smtpUser, smtpPass);
						client.EnableSsl = true;
						client.UseDefaultCredentials = false;

						await client.SendMailAsync(message);
						_logger.LogInformation($"Email sent to {email} with subject: {subject}");
					}
				}
			}
			catch (SmtpException ex)
			{
				_logger.LogError($"SMTP error sending email to {email}: {ex.Message}");
				throw new Exception("Error sending email. Please try again later.");
			}
			catch (Exception ex)
			{
				_logger.LogError($"Unexpected error sending email to {email}: {ex.Message}");
				throw new Exception("An unexpected error occurred while sending the email.");
			}
		}
	}
}
