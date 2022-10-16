using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HROtReq
    {
        public int Id { get; set; }
        public int OrgId { get; set; }
        public int No { get; set; }
       // public int AttendanceId { get; set; }
        public int HodAppBy { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public string ApprovedBy { get; set; }
        [MaxLength(200)] public string Remarks { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        public string EmployeeGroup { get; set; }

        public DateTime Date { get; set; }
        public DateTime HodAppDate { get; set; }
        public DateTime ApprovedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        //navigation property
        public HRAttendance Attendance { get; set; }
    }
}
