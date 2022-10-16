using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APIGP
    {
        [Key]
        public int Id { get; set; }
        public int IGP { get; set; }

        [Display(Name = "IGP Date")]
        public DateTime IGPDate { get; set; }
        [Display(Name ="Vendor Name")]
        public int VendorId { get; set; }
        public string DC { get; set; }
        public string Bility { get; set; }
        public string Address { get; set; }
        [Display(Name ="Vehicle No")]
        public string Vehicle { get; set; }
        public string VehicleType { get; set; }
        public string DriverName { get; set; }
        public string TransportCompany { get; set; }
        public string Remarks { get; set; }
        public int OperatingId { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int CompanyId { get; set; }
        public int Resp_ID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string UnApprovedBy { get; set; }
        public DateTime UnApprovedDate { get; set; }
        public bool IsApproved { get; set; }
        public bool IsIRN { get; set; }
        public int POTypeId { get; set; }
        public int VoucherId { get; set; }
        public int FreightTypeId { get; set; }
        public DateTime UnloadingDate { get; set; }
        public decimal FreightAmount { get; set; }
        [Display(Name ="Unloading Amount")]
        public decimal UnloadingAmount { get; set; }
        //Navigation Property
        public APSupplier Vendor { get; set; }
        public AppCompanyConfig POType { get; set; }
        public AppCompanyConfig FreightType { get; set; }
        public SysOrganization Operating { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }

        //navigational property
        [ForeignKey("CreatedBy")]
        public ApplicationUser CreatedUser { get; set; }
        [ForeignKey("ApprovedBy")]
        public ApplicationUser ApprovalUser { get; set; }
        [NotMapped]
        public string Auser { get; set; }
    }
}
