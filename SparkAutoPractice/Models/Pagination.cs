using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparkAutoPractice.Models
{
    // This class holds info for pagination on the users page
    // This file does not need to be added to the database, so it is not in the ApplicationDbContext
    public class Pagination
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
        public string UrlParam { get; set; }
    }
}
