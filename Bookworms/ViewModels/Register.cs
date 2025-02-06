using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Bookworms.ViewModels
{
    public class Register
    {
		[Required]
		[DataType(DataType.Text)]
		public string FirstName { get; set; }

		[Required]
		[DataType(DataType.Text)]
		public string LastName { get; set; }

		[Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

		[Required]
		[DataType(DataType.PhoneNumber)]
		public string MobileNo { get; set; }

		[Required]
		[MinLength(8, ErrorMessage = "Enter at least a 12 characters password")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{8,}$", 
			ErrorMessage = "Passwords must be at least 8 characters long and contain at least an uppercase letter, lower case letter, digit and a symbol")]

		[DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

		[Required]
		[DataType(DataType.CreditCard)]
		public string CreditCard { get; set; }

		[Required]
		[DataType(DataType.MultilineText)]
		public string BillingAddress { get; set; }

		[Required]
		[DataType(DataType.MultilineText)]
		public string ShippingAddress { get; set; }

		[Display(Name = "Profile Photo")]
		public IFormFile Photo { get; set; }


	}
}
