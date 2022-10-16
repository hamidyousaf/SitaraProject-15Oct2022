using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HREmployeeShortLeave
    {
        public int Id { get; set; }
        public int OrgId { get; set; }

        public int Km { get; set; }
        public int HodAppBy { get; set; }
       // public int EmployeeId { get; set; }
        public int CustomerId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public string ApprovedBy { get; set; }
        [MaxLength(200)] public string Remarks { get; set; }
        [MaxLength(10)] public string Status { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal Hours { get; set; }

        public DateTime ToDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime HodAppDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ApprovedDate { get; set; }

        //navigation property
        public HREmployee Employee { get; set; }
    }
}
