using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class YarnIssuance
     {
        public int Id { get; set; }
        [Display(Name = "Issue No.")] public int IssueNo { get; set; }
        [Display(Name = "Issue Date")] [DataType(DataType.Date)] public DateTime IssueDate { get; set; }
        [Display(Name = "Ware House")] public int WareHouseId { get; set; }
        [Display(Name = "Vendor")] public int VendorId { get; set; }
        [Display(Name = "Weaving Contract #")] public int WeavingContractId { get; set; }
 
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
        public APSupplier Vendor { get; set; }
        public ApplicationUser User { get; set; }
        public GRWeavingContract WeavingContract { get; set; }
        public virtual ICollection<WarpIssuance> WarpIssuances { get; set; }
        public virtual ICollection<WeftIssuance> WeftIssuances { get; set; }
    }
}
