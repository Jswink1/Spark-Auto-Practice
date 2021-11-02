using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SparkAutoPractice.Data;

namespace SparkAutoPractice.Areas.Identity.Pages.Account
{
    public class VerifyEmailModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public VerifyEmailModel(ApplicationDbContext db, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _db = db;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public string Email { get; set; }

        public IActionResult OnGet(string id)
        {
            Email = id;

            return Page();
        }

        public async Task<IActionResult> OnPostResendEmail()
        {
            var user = _db.ApplicationUser.Where(x => x.Email == Email).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }


            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code, returnUrl = Url.Content("~/") },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return Page();
        }
    }
}
