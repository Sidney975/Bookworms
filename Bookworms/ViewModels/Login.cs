using System.ComponentModel.DataAnnotations;

namespace Bookworms.ViewModels
{
    public class Login
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
		public string? RecaptchaToken { get; set; }
	}

	public class RecaptchaResponse
	{
		public bool success { get; set; }
		public float score { get; set; }
		public string action { get; set; }
		public string challenge_ts { get; set; }
		public string hostname { get; set; }
		public List<string> error_codes { get; set; }
	}
}
