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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
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

            Customer = await _db.ApplicationUser.FirstOrDefaultAsync(x => x.Id == id);

            if (Customer == null)
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
            else
            {
                var userFromDb = await _db.ApplicationUser.SingleOrDefaultAsync(x => x.Id == Customer.Id);

                if (userFromDb == null)
                {
                    return NotFound();
                }
                else
                {
                    userFromDb.Name = Customer.Name;
                    userFromDb.PhoneNumber = Customer.PhoneNumber;
                    userFromDb.Address = Customer.Address;
                    userFromDb.City = Customer.City;
                    userFromDb.PostalCode = Customer.PostalCode;

                    await _db.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
            }
        }
    }
}
