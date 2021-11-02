using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SparkAutoPractice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SparkAutoPractice.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            // Retrieve ClaimObject in order to tell if a user is logged in
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // If no user is logged in, redirect to Login
            if (claim == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // If admin has logged in, redirect to Users Page
            if (User.IsInRole(SD.AdminEndUser))
            {
                return RedirectToPage("/Users/Index");
            }

            // If customer has logged in, Redirect to Cars Page
            return RedirectToPage("/Cars/Index");
        }
    }
}
