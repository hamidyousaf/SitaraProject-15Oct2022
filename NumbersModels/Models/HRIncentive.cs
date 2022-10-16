using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRIncentive
    {
        public int Id { get; set; }
        public int No { get; set; }
        public int OrgId { get; set; }
       // public int EmployeeTypeId { get; set; }
        public int CompanyId { get; set; }
        public int SystemId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string System { get; set; }
        [MaxLength(200)] public string Remarks { get; set; }
        public string PieceRate { get; set; }
        public string Type { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        public string Closed { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal Amount { get; set; }

        public DateTime ToDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime Date { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ApprovedDate { get; set; }

        //navigation property
        public HREmployeeType EmployeeType { get; set; }
    }
}
