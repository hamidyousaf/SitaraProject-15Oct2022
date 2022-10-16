using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRAttendance
    {
        public int Id { get; set; }
        public int DateId { get; set; }
        //public int ShiftId { get; set; }
        public int OrgId { get; set; }
        //public int EmployeeId { get; set; }

        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        [MaxLength(1)] public string Chk { get; set; }
        [MaxLength(20)] public string TimeStatus { get; set; }
        [MaxLength(200)] public string Remarks { get; set; }//250
        [MaxLength(10)] public string Project { get; set; }//20
        [MaxLength(100)] public string Pr { get; set; }
        [MaxLength(1)] public string Posted { get; set; }
        [MaxLength(100)] public string AutoA { get; set; }


        // [Column(TypeName = "decimal(18, 2)")]
        [Column(TypeName = "numeric(18,2)")] public decimal OtHrs { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Leaves { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal LateNo { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal LateDed { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal Hours { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal GrossDedHours { get; set; }
        [Column(TypeName = "numeric(18,2)")] public decimal BasicDedHours { get; set; }


        public TimeSpan AbsentTime { get; set; }
        public TimeSpan BreakStartTime { get; set; }
        public TimeSpan BreakEndTime { get; set; }
        public TimeSpan BreakOut { get; set; }
        public TimeSpan BreakIn { get; set; }
        public TimeSpan HalfDayTime { get; set; }
        public TimeSpan HalfDayShortTime { get; set; }
        public TimeSpan InTime { get; set; }
        public TimeSpan OutTime { get; set; }
        public TimeSpan LateTime { get; set; }
        public TimeSpan ShortLeaveTime { get; set; }
        public TimeSpan ShiftStartTime { get; set; }
        public TimeSpan ShiftEndTime { get; set; }


        public DateTime TransDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        

        //navigation property
        public HREmployee Employee { get; set; }
        public HRShift Shift { get; set; }
    }
}
