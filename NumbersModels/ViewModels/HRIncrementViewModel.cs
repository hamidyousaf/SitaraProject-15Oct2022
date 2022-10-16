using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class HRIncrementViewModel
    {
        //HRIncrement
        public int Id { get; set; }
        [Display(Name = "Increment No")] public int IncrementNo { get; set; }
        [Display(Name = "Increment Date")] public DateTime IncrementDate { get; set; }
        [Display(Name = "Increment Type")] [MaxLength(450)] public string IncrementType { get; set; }
        [Display(Name = "Department")] [MaxLength(450)] public string Department { get; set; }
        [Display(Name = "Designation")] [MaxLength(450)] public string Designation { get; set; }
        [Display(Name = "Employee Type")]  public int EmployeeType { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        [Display(Name = "Increment %")] public decimal IncrementPercentage { get; set; }
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

        //HRIncrementItems
        [Display(Name = "Increment Item Id")] public int IncrementItemId { get; set; }
        [Display(Name = "Increment Id")] public int IncrementId { get; set; }
        [Display(Name = "Employee Id")] public int EmployeeId { get; set; }

        [Display(Name = "Promotion")] public string Promotion { get; set; }
        [Display(Name = "Old Grade")] public string OldGrade { get; set; }
        [Display(Name = "Old Designation")] public string OldDesignation { get; set; }
        [Display(Name = "New Grade")] public string NewGrade { get; set; }
        [Display(Name = "New Designation")] public string NewDesignation { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal CurrentPay { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Amount { get; set; }

        //navigation property
        public HRIncrement Increment { get; set; }
        public HREmployee Employee { get; set; }

        public List<HREmployeeType> EmployeeTypes { get; set; }
    }
}
