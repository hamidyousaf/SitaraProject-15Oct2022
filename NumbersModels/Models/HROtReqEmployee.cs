using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HROtReqEmployee
    {
        public int Id { get; set; }
        public int OtReqId { get; set; }
        // public int EmployeeId { get; set; }
        //public int AttendanceId { get; set; }
        // public int ShiftId { get; set; }
        public int DeptId { get; set; }
        public int DesgSalary { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public string ODesignation { get; set; }
        public string Cpl { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        public string Tsot { get; set; }
        [MaxLength(200)] public string Remarks { get; set; }
        public string Designation { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal DeptOt { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal ActualOt { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Rate { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal CurrentPay { get; set; }

        public TimeSpan TimeIn { get; set; }
        public TimeSpan TimeOut { get; set; }

        public DateTime DeptOtStart { get; set; }
        public DateTime DeptOtEnd { get; set; }
        public DateTime ActualOtStart { get; set; }
        public DateTime ActualOtEnd { get; set; }
       
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }


        //navigation property
        public HREmployee Employee { get; set; }
        public HRShift Shift { get; set; }
        public HRAttendance Attendance { get; set; }
    }
}
