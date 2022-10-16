using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRShiftChange
    {
        public int Id { get; set; }
        // public int EmployeeId { get; set; }
        //public int NewShiftId { get; set; }
        public int OldShiftId { get; set; }


        [MaxLength(250)] public string EmployeeGroup { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }


        //navigation property
        public HREmployee Employee { get; set; }
        public HRShift NewShift { get; set; }

    }
}
