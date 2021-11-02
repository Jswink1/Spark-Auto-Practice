using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SparkAutoPractice.Models
{
    public class ServiceHeader
    {
        public int Id { get; set; }
        public double Miles { get; set; }
        public string Details { get; set; }
        public int CarId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:MMM dd yyyy}")]
        public DateTime DateAdded { get; set; }

        [Required]
        public double TotalPrice { get; set; }

        [ForeignKey("CarId")]
        public virtual Car Car { get; set; }
    }
}
