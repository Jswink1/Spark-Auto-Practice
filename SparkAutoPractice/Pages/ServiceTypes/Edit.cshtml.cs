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

namespace SparkAutoPractice.Pages.ServiceTypes
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
        public ServiceType ServiceType { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ServiceType = await _db.ServiceType.FirstOrDefaultAsync(s => s.Id == id);

            if (ServiceType == null)
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

            // This method updates all the properties of the ServiceType entry.
            // If you want to update only specific properties in order to be more efficient, use the method in the comments at the bottom
            _db.Attach(ServiceType).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceTypeExists(ServiceType.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //var serviceFromDb = await _db.ServiceType.FirstOrDefaultAsync(x => x.Id == ServiceType.Id);
            //serviceFromDb.Name = ServiceType.Name;
            //serviceFromDb.Price = ServiceType.Price;
            //await _db.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool ServiceTypeExists(int id)
        {
            return _db.ServiceType.Any(e => e.Id == id);
        }
    }
}
