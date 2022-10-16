using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRRouster
    {
        public int Id { get; set; }
        public string PeriodId { get; set; }
        public int GroupId { get; set; }

        [MaxLength(100)] public bool Closed { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; } 
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
       
    }
}
