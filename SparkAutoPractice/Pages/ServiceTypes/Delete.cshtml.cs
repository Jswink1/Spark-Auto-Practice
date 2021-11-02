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
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;        

        public DeleteModel(ApplicationDbContext db)
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

        public async Task<IActionResult> OnPostDelete()
        {
            var service = await _db.ServiceType.FindAsync(ServiceType.Id);

            if (service == null)
            {
                return NotFound();
            }

            _db.ServiceType.Remove(service);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
