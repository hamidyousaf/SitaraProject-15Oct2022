using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRAttendanceLog
    {
        public int Id { get; set; }
        //public int EmployeeId { get; set; }

        [MaxLength(100)] public string EmployeeNo { get; set; }
        [MaxLength(50)] public string Cancel { get; set; }
        [MaxLength(50)] public string Posted { get; set; }
        [MaxLength(50)] public string TransType { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }

        public TimeSpan TransTime { get; set; }

        public DateTime AttendanceDate { get; set; }

        //navigation property
        public HREmployee Employee { get; set; }
    }
}
