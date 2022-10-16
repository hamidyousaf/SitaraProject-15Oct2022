using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREmployeeLeaveOpening
    {
        public int Id { get; set; }
        [Display(Name = "Opening No.")] public int No { get; set; }
        [Display(Name = "Leave Type")] public int LeaveTypeId { get; set; }
        //public int LeaveId { get; set; }
        [Required] [Display(Name = "Employee Id")] public int EmployeeId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public string ApprovedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Required] [MaxLength(10)] public string Status { get; set; }

        [Required] [Display(Name = "Opening Balance")] [Column(TypeName = "numeric(18,2)")] public decimal OpeningBalance { get; set; }

        public bool IsDeleted { get; set; }
        [Display(Name = "Active")] public bool IsActive { get; set; }

        [Display(Name = "Opening Date")] public DateTime OpeningDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Approved Date")] public DateTime ApprovedDate { get; set; }

        //navigation property
        public HRLeave Leave { get; set; }
        public HREmployee Employee { get; set; }
    }
}
