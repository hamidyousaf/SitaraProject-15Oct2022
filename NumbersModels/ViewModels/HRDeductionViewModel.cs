using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRDeductionViewModel
    {

        // HR Deduction
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int DeductionNo { get; set; }
        public int EmployeeTypeId { get; set; }
        public int DeductionID { get; set; }

        [MaxLength(200)] public string Remarks { get; set; }
        [MaxLength(200)] public string Type { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        [MaxLength(200)] public string Closed { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal OrgId { get; set; }

        public DateTime Date { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsSystem { get; set; }
        public bool IsDeleted { get; set; }

        public int EmployeeId { get; set; }
        public int DeductionId { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal Amount { get; set; }
        //navigation property
        public HREmployee Employee { get; set; }
        public HREmployeeType EmployeeType { get; set; }
        public HRDeduction Deduction { get; set; }

    }
}
