using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class APOGP
    {
        [Key]
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime OGPDate { get; set; }
        public int OGPNo { get; set; }
        public int VendorID { get; set; }
        public int PONo { get; set; }
        public int IRNId { get; set; }
        public string Bility { get; set; }
        public string Vehicle { get; set; }
        public string VehicleType { get; set; }
        public string DriverName { get; set; }
        public string TransportCompany { get; set; }
        public string Remarks { get; set; }
        public int OperatingId { get; set; }
        [ForeignKey("User")]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
        public int Resp_ID { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }
        [ForeignKey("ApprovedUser")]
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string UnApprovedBy { get; set; }
        public DateTime UnApprovedDate { get; set; }
        public bool IsActive { get; set; }
        //Navigation Property
        public APSupplier Vendor { get; set; }
        public APIRN IRN { get; set; }
        public ApplicationUser User { get; set; }
        public ApplicationUser ApprovedUser { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }
        [NotMapped]
        public string ApprovalUser { get; set; }
    }
}