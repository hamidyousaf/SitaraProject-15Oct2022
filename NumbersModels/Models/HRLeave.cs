using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRLeave
    {
        public int Id { get; set; }
        public int LeaveTypeId { get; set; }

        [Required] [Display(Name = "Description")] [MaxLength(200)] public string Description { get; set; }
        [Required] [Display(Name = "Short Description")] [MaxLength(100)] public string ShortDescription { get; set; }
        [Required] [Display(Name = "Unit")] public string Unit { get; set; }
        [Required] [Display(Name = "Flag")] public string Flag { get; set; }

        [Required] [Range(0, 999.99)] [Display(Name = "Weight")] [Column(TypeName = "numeric(18,2)")] public decimal Weight { get; set; }

        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        [Display(Name = "Active")] public bool IsActive { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        //navigation property
        public HRLeaveType LeaveType { get; set; }
    }
}
