using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
   public class APPurchaseRequisition
    {
        [Key]
        public int Id { get; set; }
        public DateTime PrDate { get; set; }
        public string Attachment { get; set; }
        public string Remarks { get; set; }
        public string RefrenceNo { get; set; }

        [Display(Name ="Pr No")]
        public string PrNo { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }
        public int OperationId { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
        public int Resp_ID { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }
        [NotMapped]
        public string Auser { get; set; }
        public DateTime ApprovedDate { get; set; }
        public DateTime UnApprovedDate { get; set; }
        public string UnApprovedBy { get; set; }
        public bool IsApproved { get; set; }
        public int RequisitionTypeId { get; set; }
        [Display(Name ="G.R#")]
        public int GreigeRequisitionId { get; set; }
        //navigational property
        public GLDivision Department { get; set; }
        public AppCompanyConfig RequisitionType { get; set; }
        public SysOrganization Operation { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }

        //navigational property
        [ForeignKey("CreatedBy")]
        public ApplicationUser CreatedUser { get; set; }
        [ForeignKey("ApprovedBy")]
        public ApplicationUser ApprovalUser { get; set; }
    }

}
