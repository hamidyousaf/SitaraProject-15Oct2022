using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
     public class HREmployeeFamily
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }      
      
        [Display(Name = "Relative Name")] [MaxLength(50)] public string RelativeName { get; set; }
        [Display(Name = "Relation")] public string Relation { get; set; }

        public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime Dob { get; set; }

        //navigation property
        public HREmployee Employee { get; set; }
    }
}
