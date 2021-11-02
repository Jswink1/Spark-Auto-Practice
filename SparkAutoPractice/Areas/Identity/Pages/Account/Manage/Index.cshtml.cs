using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAutoPractice.Data;

namespace SparkAutoPractice.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Name { get; set; }

            public string Address { get; set; }
            public string City { get; set; }
            public string PostalCode { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            // Retrieve user info from DB
            var userFromDb = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Email == user.Email);
            Username = userFromDb.UserName;

            // Display user info in input boxes
            Input = new InputModel
            {
                Email = userFromDb.Email,
                PhoneNumber = userFromDb.PhoneNumber,
                Address = userFromDb.Address,
                City = userFromDb.City,
                Name = userFromDb.Name,
                PostalCode = userFromDb.PostalCode
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Save new user info changes
            var userFromDb = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Email == user.Email);
            userFromDb.Name = Input.Name;
            userFromDb.Address = Input.Address;
            userFromDb.City = Input.City;
            userFromDb.PostalCode = Input.PostalCode;
            userFromDb.PhoneNumber = Input.PhoneNumber;
            await _db.SaveChangesAsync();

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
