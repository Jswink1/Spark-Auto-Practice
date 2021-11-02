using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAutoPractice.Data;
using SparkAutoPractice.Models;

namespace SparkAutoPractice.Pages.Cars
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Car Car { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Car = await _db.Car.Include(c => c.ApplicationUser).FirstOrDefaultAsync(x => x.Id == id);

            if (Car == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            _db.Attach(Car).State = EntityState.Modified;

            await _db.SaveChangesAsync();
            StatusMessage = "Car updated successfully.";

            // userId must be passed here, because if an admin is logged in,
            // we do not want to pass the Id of the admin, we want to pass the Id of the customer who the car belongs to
            return RedirectToPage("./Index", new { userId = Car.UserId });
        }
    }
}
