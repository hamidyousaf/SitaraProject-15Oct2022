using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRFinalSettlementLeave
    {
        public int Id { get; set; }
        //public int FinalSettlementId { get; set; }
        //public int LeaveTypeGroupId { get; set; }
        //public int LeaveId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal NumberOfDays { get; set; }

        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        //navigation property
        public HRFinalSettlement FinalSettlement { get; set; }
        public HRLeaveTypeGroup LeaveTypeGroup { get; set; }
        public HRLeave Leave { get; set; }
    }
}
