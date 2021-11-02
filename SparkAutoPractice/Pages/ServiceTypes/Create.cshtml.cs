using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SparkAutoPractice.Data;
using SparkAutoPractice.Models;
using SparkAutoPractice.Utility;

namespace SparkAutoPractice.Pages.ServiceTypes
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public ServiceType ServiceType { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            _db.ServiceType.Add(ServiceType);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
