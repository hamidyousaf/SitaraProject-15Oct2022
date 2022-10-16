using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREncashmentLeave
    {
        public int Id { get; set; }
        public int NoOfLeaves { get; set; }
        //public int LeaveTypeId { get; set; }
        //public int EncashmentId { get; set; }
        public int LeaveAccountId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal LeaveAmount { get; set; }

        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        //navigation property
        public HRLeaveType LeaveType { get; set; }
        public HREncashment Encashment { get; set; }
    }
}
