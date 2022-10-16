using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
   public class HREmployeeExperience
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }
      
        public string Position { get; set; }
        public string Duration { get; set; }
        public string Company { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal Pay { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime ResigningDate { get; set; }
        public DateTime JoiningDate { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }  

        //navigation property
        public HREmployee Employee { get; set; }
    }
}
