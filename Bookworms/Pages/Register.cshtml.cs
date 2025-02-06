using Bookworms.Models;
using Bookworms.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookworms.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }
        private readonly RoleManager<IdentityRole> roleManager;
		private readonly IWebHostEnvironment webHostEnvironment;
		[BindProperty]
        public Register RModel { get; set; }
        public RegisterModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
		IWebHostEnvironment webHostEnvironment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.webHostEnvironment = webHostEnvironment;

		}

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");

                var user = new ApplicationUser()
                {
                    FirstName = RModel.FirstName,
					LastName = RModel.LastName,
					UserName = RModel.Email,
                    Email = RModel.Email,
                    PhoneNumber = RModel.MobileNo,
                    CreditCard = protector.Protect(RModel.CreditCard),
                    Billing_Address = RModel.BillingAddress,
                    Shipping_Address = RModel.ShippingAddress,
                };

				if (RModel.Photo != null && RModel.Photo.Length > 0)
				{
					// Determine the uploads folder path (e.g., wwwroot/uploads)
					var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
					if (!Directory.Exists(uploadsFolder))
					{
						Directory.CreateDirectory(uploadsFolder);
					}

					// Generate a unique file name (e.g., using a GUID)
					var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(RModel.Photo.FileName)}";
					var filePath = Path.Combine(uploadsFolder, uniqueFileName);

					// Save the file to disk
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await RModel.Photo.CopyToAsync(fileStream);
					}

					// Save the file name (or relative path) to the user record.
					// You can later use this to display the photo.
					user.Photo = uniqueFileName;
				}

				//Create the Admin role if NOT exist
				IdentityRole role = await roleManager.FindByIdAsync("Admin");
                if (role == null)
                {
                    IdentityResult result2 = await roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!result2.Succeeded)
                    {
                        ModelState.AddModelError("", "Create role admin failed");
                    }
                }


                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    //Add users to Admin Role
                    result = await userManager.AddToRoleAsync(user, "Admin");


                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("",
                error.Description);
                }
            }
            return Page();
        }


        public void OnGet()
        {
        }
    }
}
