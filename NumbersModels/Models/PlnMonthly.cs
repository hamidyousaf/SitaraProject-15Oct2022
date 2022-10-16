using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class PlnMonthly
    {
        public int Id { get; set; }
        [Display(Name = "Trans No.")] public int IssueNo { get; set; }
        [Display(Name = "Transaction Date")] [DataType(DataType.Date)] public DateTime IssueDate { get; set; }
        [Display(Name = "Seasonal Planning")] public int SeasonId { get; set; }
        [Display(Name = "Plan Of")] public int MonthId { get; set; }
        [Display(Name = "Department")] public int DepartmentId { get; set; }
        [Display(Name = "Sub Department")] public int SubDepartmentId { get; set; }
        [Display(Name = "Status")] public string Status { get; set; }
        [Display(Name = "Remarks")] [MaxLength(400)] public string Remarks { get; set; }
        public int VoucherId { get; set; }
        public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)][ForeignKey("User")] public string CreatedBy { get; set; }
        [Display(Name = "Created Date")] [DataType(DataType.Date)] public DateTime? CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Updated Date")] [DataType(DataType.Date)] public DateTime? UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [Display(Name = "Approved Date")] [DataType(DataType.Date)] public DateTime? ApprovedDate { get; set; }

        //public AppCompanyConfig WareHouse { get; set; }

        public AppCompanyConfigBase Season { get; set; }
        //public AppCompanyConfigBase Month { get; set; }

        public CostCenter CostCenter { get; set; }
        public ApplicationUser User { get; set; }
    }
}
