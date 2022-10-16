using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class APIRN
    {
        [Key]
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime IRNDate { get; set; }
        public int IRNNo { get; set; }
        public int VendorID { get; set; }
        public int PONo { get; set; }
        public int IGPNo { get; set; }
        public string Remarks { get; set; }
        public int OperatingId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
        public int Resp_ID { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string UnApprovedBy { get; set; }
        public DateTime UnApprovedDate { get; set; }
        public bool IsOGP { get; set; }
        public bool IsActive { get; set; }
        //Navigation Property
        public APSupplier Vendor { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
    }
}