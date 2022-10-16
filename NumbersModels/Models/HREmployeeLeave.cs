using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREmployeeLeave
    {
        public int Id { get; set; }
        public int SubstitudeId { get; set; }
        public int SecAppBy { get; set; }
        public int OrgId { get; set; }
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }
        public int HodAppBy { get; set; }


        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public string ApprovedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [MaxLength(200)] public string Remarks { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        public string ContactNo { get; set; }
        public string ContactAddress { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal NoOfDays { get; set; }

        public DateTime ToDate { get; set; }
        public DateTime SecAppDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime HodAppDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ApprovedDate { get; set; }

        //navigation property
        public HRLeave Leave { get; set; }
        public HREmployee Employee { get; set; }
    }
}
