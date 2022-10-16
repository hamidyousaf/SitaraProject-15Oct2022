using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRAttendanceDate
    {
        public int Id { get; set; }
        //public int AttendanceId { get; set; }

        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public string Processed { get; set; }
        public string Posted { get; set; }

        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime AttendanceDate { get; set; }

        //navigation property
        public HRAttendance Attendance { get; set; }
    }
}
