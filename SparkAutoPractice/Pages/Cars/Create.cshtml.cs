using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SparkAutoPractice.Data;
using SparkAutoPractice.Models;

namespace SparkAutoPractice.Pages.Cars
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;      

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Car Car { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult OnGet(string userId = null)
        {
            Car = new();

            // userId will be null if user is not an admin user,
            // so ClaimsIdentity is used to populate the user identity
            if (userId == null)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                userId = claim.Value;
            }
            Car.UserId = userId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            _db.Car.Add(Car);
            await _db.SaveChangesAsync();
            StatusMessage = "Car has been added successfully.";

            // userId must be passed here, because if an admin is logged in,
            // we do not want to pass the Id of the admin, we want to pass the Id of the customer who the car belongs to
            return RedirectToPage("Index", new { userId = Car.UserId });
        }
    }
}
