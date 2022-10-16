using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRGriegeRequisitionST
    {
        public int Id { get; set; }
        public int TransactionNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public int DepartmentId { get; set; }
        public int SubDepartmentId { get; set; }
        public int SPID { get; set; }
        public int Resp_ID { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsUsed { get; set; }
        public string Deletedby { get; set; }
        public bool IsApproved { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string UnApprovedBy { get; set; }
        public string OrderRef { get; set; }
        public string Remarks { get; set; }
        public DateTime UnApprovedDate { get; set; }
        public DateTime DeletedDate { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
        [Display(Name = "Create PR")]
        public bool IsCreatePR { get; set; }
        public GLDivision Department { get; set; }
        public GLSubDivision SubDepartment { get; set; }
    }
}
