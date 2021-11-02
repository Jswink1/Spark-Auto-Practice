using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAutoPractice.Data;
using SparkAutoPractice.Models;
using SparkAutoPractice.Models.ViewModels;
using SparkAutoPractice.Utility;

namespace SparkAutoPractice.Pages.Users
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public UsersPaginationViewModel UsersPaginationVM { get; set; }

        public async Task<IActionResult> OnGet(int usersPage = 1, string searchEmail = null, string searchName = null, string searchPhone = null)
        {
            // Retrieve List of Users
            UsersPaginationVM = new()
            {
                ApplicationUsers = await _db.ApplicationUser.ToListAsync()
            };

            // Create url parameters
            StringBuilder url = new();
            url.Append("/Users?usersPage=:");

            // Determine if there is search criteria
            url.Append("&searchEmail=");
            if (searchEmail != null)
            {
                url.Append(searchEmail);
            }
            url.Append("&searchName=");
            if (searchName != null)
            {
                url.Append(searchName);
            }
            url.Append("&searchPhone=");
            if (searchPhone != null)
            {
                url.Append(searchPhone);
            }

            // Apply filter to search results
            if (searchEmail != null)
            {
                UsersPaginationVM.ApplicationUsers = await _db.ApplicationUser.Where(x => x.Email.ToLower().Contains(searchEmail.ToLower())).ToListAsync();
            }
            else
            {
                if (searchName != null)
                {
                    UsersPaginationVM.ApplicationUsers = await _db.ApplicationUser.Where(x => x.Name.ToLower().Contains(searchName.ToLower())).ToListAsync();
                }
                else
                {
                    if (searchPhone != null)
                    {
                        UsersPaginationVM.ApplicationUsers = await _db.ApplicationUser.Where(x => x.PhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToListAsync();
                    }
                }
            }

            // Find total number of user records
            var count = UsersPaginationVM.ApplicationUsers.Count;

            // Initialize Pagination properties
            UsersPaginationVM.Pagination = new()
            {
                CurrentPage = usersPage,
                ItemsPerPage = SD.PaginationUsersPerPage,
                TotalItems = count,
                UrlParam = url.ToString()
            };

            // Retrieve the records that belong on the currently selected page
            UsersPaginationVM.ApplicationUsers = UsersPaginationVM.ApplicationUsers.OrderBy(x => x.Email)
                .Skip((usersPage - 1) * SD.PaginationUsersPerPage)
                .Take(2).ToList();

            return Page();
        }
    }
}
