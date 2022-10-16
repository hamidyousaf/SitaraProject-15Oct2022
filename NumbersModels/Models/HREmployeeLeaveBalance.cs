using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREmployeeLeaveBalance
    {
        public int Id { get; set; }
        public int PeriodId { get; set; }
        public int ModuleDocId { get; set; }
        // public int LeaveId { get; set; }
        // public int EmployeeId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public string Posted { get; set; }
        public string Module { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal Balance { get; set; }

        public DateTime ToDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime CreatedDate { get; set; }

        //navigation property
        public HRLeave Leave { get; set; }
        public HREmployee Employee { get; set; }
    }
}
