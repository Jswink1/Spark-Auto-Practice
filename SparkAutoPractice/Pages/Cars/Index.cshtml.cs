using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAutoPractice.Data;
using SparkAutoPractice.Models.ViewModels;

namespace SparkAutoPractice.Pages.Cars
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public CarAndCustomerViewModel CarAndCustomerVM { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet(string userId = null)
        {
            // userId will be null if current user is not an admin user,
            // so ClaimsIdentity is used to populate the user id
            if (userId == null)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                userId = claim.Value;
            }

            CarAndCustomerVM = new()
            {
                Cars = await _db.Car.Where(c => c.UserId == userId).ToListAsync(),
                User = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == userId)
            };

            return Page();
        }
    }
}
