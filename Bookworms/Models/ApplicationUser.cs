using Microsoft.AspNetCore.Identity;

namespace Bookworms.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? CreditCard { get; set; }
        public string? Billing_Address { get; set; }
		public string? Shipping_Address { get; set; }
		public string? Photo { get; set; }

        public string? CurrentSessionId { get; set; }

		public DateTime LastPasswordChangeDate { get; set; } = DateTime.UtcNow;


	}

}
