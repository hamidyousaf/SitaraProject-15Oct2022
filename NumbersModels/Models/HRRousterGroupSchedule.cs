using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRRousterGroupSchedule
    {
        public int Id { get; set; }
        //public int RousterId { get; set; }
        //public int ShiftId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public string GroupId { get; set; }
        public string PeriodId { get; set; }

        public DateTime RousterDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        //navigation property
        public HRShift Shift { get; set; }
        public HRRouster Rouster { get; set; }
        public int RousterId { get; set; }
        public int ShiftId { get; set; }
    }
}
