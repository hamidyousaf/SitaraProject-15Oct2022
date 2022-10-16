using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRGazetedHoliday
    {
        public int Id { get; set; }
        public int No { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)]  public string UpdatedBy { get; set; }
        public string ApprovedBy { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        [MaxLength(200)] public string Description { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal NoOfDays { get; set; }

        public DateTime ToDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ApprovalDate { get; set; }
    }
}
