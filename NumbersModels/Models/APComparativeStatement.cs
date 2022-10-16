using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APComparativeStatement
    {
        public int ID { get; set; }

        [Display(Name = "Operating Unit")]
        public int OperatingId { get; set; }

        [Display(Name ="CS #")]
        public int CS { get; set; }
        public DateTime CSDate { get; set; }

        [Display(Name = "CS Validity")]
        public DateTime CSValidity { get; set; }
        public string Remarks { get; set; }
        public string Attachment { get; set; }
        public int DepartmentId { get; set; }
        [ForeignKey("User")]

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
        public int Resp_ID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public bool IsApprove { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string UnApprovedBy { get; set; }
        public DateTime UnApprovedDate { get; set; }
        public bool IsPoCreated { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
        //navigational property
         
        public SysOrganization Operating { get; set; }
        public ApplicationUser User { get; set; }
        [ForeignKey("ApprovedBy")]
        public ApplicationUser ApprovalUser { get; set; }
        [NotMapped]
        public string Auser { get; set; }
    }
}
