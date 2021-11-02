using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparkAutoPractice.Models.ViewModels
{
    public class UsersPaginationViewModel
    {
        public List<ApplicationUser> ApplicationUsers { get; set; }
        public Pagination Pagination { get; set; }
    }
}
