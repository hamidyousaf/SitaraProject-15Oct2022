using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRAttendanceMannual
    {
        public int Id { get; set; }
        //  public int EmployeeId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }

        public TimeSpan OutTime { get; set; }
        public TimeSpan InTime { get; set; }

        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime AttendanceDate { get; set; }

        //navigation property
        public HREmployee Employee { get; set; }
    }
}
