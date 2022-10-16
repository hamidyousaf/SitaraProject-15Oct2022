using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRDeduction
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int DeductionNo { get; set; }
        public int EmployeeTypeId { get; set; }

        [MaxLength(200)] public string Remarks { get; set; }
        [MaxLength(200)] public string Type { get; set; }
        [MaxLength(10)] public string Status { get; set; }
        [MaxLength(200)] public string Closed { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal OrgId { get; set; }

        public DateTime Date { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsSystem { get; set; }
        public bool IsDeleted { get; set; }
        //navigation property
        public HREmployeeType EmployeeType { get; set; }
  
    }
}
