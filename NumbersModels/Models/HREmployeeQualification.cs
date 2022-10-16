using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREmployeeQualification
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [MaxLength(4)]public string PassingYear { get; set; }
        public int Qualification { get; set; }
        [MaxLength(50)]public string Institute { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        //navigation property
        public HREmployee Employee { get; set; }
    }
}
