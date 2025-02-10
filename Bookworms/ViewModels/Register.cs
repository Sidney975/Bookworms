using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Bookworms.ViewModels
{
    public class Register
    {
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name must be under 50 characters.")]
        [DataType(DataType.Text)]
		public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name must be under 50 characters.")]
        [DataType(DataType.Text)]
		public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [RegularExpression(@"^\+?\d{8,15}$", ErrorMessage = "Enter a valid phone number.")]
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

        [Required(ErrorMessage = "Credit card is required.")]
        [RegularExpression(@"^\+?\d{16,20}$", ErrorMessage = "Enter a valid Credit card.")]
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
