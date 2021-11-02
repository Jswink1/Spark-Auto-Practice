using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SparkAutoPractice.Data;
using SparkAutoPractice.Models;
using SparkAutoPractice.Utility;

// In order to generate Identity Pages, you must add a New Scaffolded Identity Item to the Areas/Identity Folder.
// The selected Layout page should be "_Layout" and the data context class should be ApplicationDbContext
namespace SparkAutoPractice.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _db = db;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Name { get; set; }

            [Required]
            public string PhoneNumber { get; set; }

            public string Address { get; set; }
            public string City { get; set; }
            public string PostalCode { get; set; }
            public bool IsAdmin { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // This user model needs to be changed from IdentityUser to our ApplicationUser
                ApplicationUser user = new ApplicationUser 
                { 
                    UserName = Input.Email, 
                    Email = Input.Email,
                    Name = Input.Name,
                    Address = Input.Address,
                    City = Input.City,
                    PostalCode = Input.PostalCode,
                    PhoneNumber = Input.PhoneNumber
                };

                // If registering user is not an admin then automatically confirm their email address
                if (Input.IsAdmin == false)
                {
                    user.EmailConfirmed = true;
                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Check if roles exist in database
                    // If they dont exist, create them
                    if (await _roleManager.RoleExistsAsync(SD.AdminEndUser) == false)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
                    }
                    if (await _roleManager.RoleExistsAsync(SD.CustomerEndUser) == false)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser));
                    }

                    // Assign role to user
                    if (Input.IsAdmin)
                    {
                        await _userManager.AddToRoleAsync(user, SD.AdminEndUser);

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        return RedirectToPage("/Users/Index");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, SD.CustomerEndUser);

                        // If an admin is currently signed in when a customer is registered, do not auto sign-in the customer
                        if (User.IsInRole(SD.AdminEndUser))
                        {
                            return LocalRedirect(returnUrl); 
                        }
                        // We only want to auto-sign in if the current application user is a customer
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
