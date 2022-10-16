using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRLeaveTypeGroup
    {
        public int Id { get; set; }
        [Display(Name = "Leave Type")] public int LeaveTypeId { get; set; }
        [Display(Name = "Account Id")] public int AccountId { get; set; }

        [Required] [Display(Name = "Employee Type")] public int EmployeeTypeId { get; set; }
        [Display(Name = "Average")] public string Average { get; set; }
        [Display(Name = "Join2Join")] public string Join2Join { get; set; }

        [Required] [Display(Name = "Days")] [Column(TypeName = "numeric(18,2)")] public decimal NoOfDays { get; set; }
        public int CompanyId { get; set; }

        [Display(Name = "Carry Forward")] public bool Forwardable { get; set; }
        [Display(Name = "Active")] public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        //navigation property
        public HRLeaveType LeaveType { get; set; }

    }
}
