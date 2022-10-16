using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class InvStoreIssue
    {
        public int Id { get; set; }
        [Display(Name = "Issue No.")] public int IssueNo { get; set; }
        [Display(Name = "Issue Date")] [DataType(DataType.Date)] public DateTime IssueDate { get; set; }
        [Display(Name = "Ware House")] public int WareHouseId { get; set; }
        [Display(Name = "Cost Center")] public int CostCenterId { get; set; }
        [Display(Name = "Department")] public int DepartmentId { get; set; }
        [Display(Name = "Sub Department")] public int SubDepartmentId { get; set; }
        [Display(Name = "Status")] public string Status { get; set; }
        [Display(Name = "Remarks")] [MaxLength(400)] public string Remarks { get; set; }
        [Display(Name = "Transaction Type")] [MaxLength(15)] public string TransactionType { get; set; }
        public int VoucherId { get; set; }
        public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)][ForeignKey("User")] public string CreatedBy { get; set; }
        [Display(Name = "Created Date")] [DataType(DataType.Date)] public DateTime? CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Updated Date")] [DataType(DataType.Date)] public DateTime? UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        [Display(Name = "Approved Date")] [DataType(DataType.Date)] public DateTime? ApprovedDate { get; set; }

        public AppCompanyConfig WareHouse { get; set; }
        public CostCenter CostCenter { get; set; }
        public ApplicationUser User { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }

        [ForeignKey("DepartmentId")]
        public GLDivision Department { get; set; }

        [ForeignKey("SubDepartmentId")]
        public GLSubDivision SubDepartment { get; set; }


    }
}
