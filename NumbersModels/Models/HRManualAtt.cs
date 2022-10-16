using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRManualAtt
    {
        public int Id { get; set; }
       // public int AttendanceId { get; set; }
       // public int EmployeeId { get; set; }
        public int CheckedBy { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public string ApprovedBy { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        [MaxLength(200)] public string Remarks { get; set; }

        public TimeSpan InTime { get; set; }
        public TimeSpan OutTime { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CheckedDate { get; set; }
        public DateTime ApprovalDate { get; set; }
        public DateTime AttendanceDate { get; set; }

        //navigation property
        public HRAttendance Attendance { get; set; }
        public HREmployee Employee { get; set; }
    }
}
