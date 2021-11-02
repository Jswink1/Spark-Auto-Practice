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
using SparkAutoPractice.Utility;

namespace SparkAutoPractice.Pages.Cars
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;        

        public DeleteModel(ApplicationDbContext db)
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

        public async Task<IActionResult> OnPostDelete()
        {
            if (Car == null)
            {
                return NotFound();
            }

            var userId = Car.UserId;

            _db.Car.Remove(Car);
            await _db.SaveChangesAsync();
            StatusMessage = "Car deleted successfully.";
            return RedirectToPage("./Index", new { userId });
        }
    }
}
