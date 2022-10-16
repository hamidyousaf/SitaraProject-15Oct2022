using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRLeaveType
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Display(Name = "Job Description")]  [MaxLength(200)] public string Description { get; set; }
        [Display(Name = "Short Description")] [MaxLength(100)]  public string ShortDescription { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [MaxLength(100)] public string Unit { get; set; }
         
        public bool IsDeleted { get; set; }
        [Display(Name = "Active")] public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
