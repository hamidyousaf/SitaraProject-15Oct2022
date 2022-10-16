using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRIncrement
    {
        public int Id { get; set; }
        [Display(Name = "Increment No")] public int IncrementNo { get; set; }
        [Display(Name = "Increment Date")] public DateTime IncrementDate { get; set; }
        [Display(Name = "Increment Type")] [MaxLength(450)] public string IncrementType { get; set; }
        [Display(Name = "Department")] [MaxLength(450)] public string Department { get; set; }
        [Display(Name = "Designation")] [MaxLength(450)] public string Designation { get; set; }
        [Display(Name = "Employee Type")]  public int EmployeeType { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        [Display(Name = "Increment %")]  public decimal IncrementPercentage { get; set; }
        [Display(Name = "Attachments")] [MaxLength(450)] public string Attachment { get; set; }

        public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }

        [MaxLength(10)] public string Status { get; set; }
        //public string Type { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }

        //navigation property




    }
}
