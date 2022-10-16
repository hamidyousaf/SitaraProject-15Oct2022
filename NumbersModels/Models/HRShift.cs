using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Numbers.Entity.Models
{
    public class HRShift
    {
        public int Id { get; set; }

        [Required] [Display(Name = "Shift Name")] [MaxLength(50)] public string Name { get; set; }
        [Display(Name = "Shift Short Name")] [MaxLength(10)] public string ShortName { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; } 
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Required] [Display(Name = "Shift Type")] [MaxLength(100)]  public string Type { get; set; }

        [Required] [Range(0.0,99.99)] [Display(Name = "Working Hours")] [Column(TypeName = "numeric(4,2)")] public decimal WorkingHours { get; set; }

        [Display(Name = "Shift Start Time")] public TimeSpan StartTime { get; set; }
        [Display(Name = "Shift End Time")] public TimeSpan EndTime { get; set; }
        [Display(Name = "Break Start Time")] public TimeSpan BreakStartTime { get; set; }
        [Display(Name = "Break End Time")] public TimeSpan BreakEndTime { get; set; }
        [Display(Name = "Mark Absent Before")] public TimeSpan MarkAbsentBefore { get; set; }
        [Display(Name = "Mark Short Leave")] public TimeSpan MarkShortLeave { get; set; }
        [Display(Name = "Mark Half Day Short")] public TimeSpan MarkHalfDayShort { get; set; }
        [Display(Name = "Mark Late After")] public TimeSpan MarkLateAfter { get; set; }
        [Display(Name = "Mark Half Day Before")] public TimeSpan MarkHalfDayBefore { get; set; }   

        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        [Display(Name = "Active")] public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
