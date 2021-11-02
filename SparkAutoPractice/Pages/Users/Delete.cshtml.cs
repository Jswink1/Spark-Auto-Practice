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

namespace SparkAutoPractice.Pages.Users
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;        

        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public ApplicationUser Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id.Trim().Length == 0)
            {
                return NotFound();
            }

            Customer = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == id);

            if (Customer == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDelete()
        {
            var customer = await _db.ApplicationUser.SingleOrDefaultAsync(u => u.Id == Customer.Id);

            _db.ApplicationUser.Remove(customer);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
