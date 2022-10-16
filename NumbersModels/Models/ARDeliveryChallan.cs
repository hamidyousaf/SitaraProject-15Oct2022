using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARDeliveryChallan
    {
        public int Id { get; set; }
        [Display(Name = "D.C. No.")] public int DCNo { get; set; }
        [Display(Name = "D.C. Date")] public DateTime DCDate { get; set; }
        [Display(Name = "Manual D.C. No.")] public int ManualDCNo { get; set; }
        [Display(Name = "Customer")] public int CustomerId { get; set; }
        [Display(Name = "Ship To")] public int ShipToId { get; set; }
        [Display(Name = "Store")] public int Store { get; set; }
        public int BranchId { get; set; }
        public int CompanyId { get; set; }
        public int ResponsibilityId { get; set; }
        [Display(Name = "Item Group")] public int ItemGroupId { get; set; }
        [Display(Name = "Sales Category")] public int SalesCategoryId { get; set; }
        [Display(Name = "Driver Contact No.")] public string DriverContactNo { get; set; }
        [Display(Name = "Vehicle No.")] public string VehicleNo { get; set; }
        [Display(Name = "Vehicle Type")] public int VehicleType { get; set; }
        [Display(Name = "Driver Name")] [MaxLength(50)] public string DriverName { get; set; }
        [Display(Name = "Transport Company")] [MaxLength(50)] public string TransportCompany { get; set; }
        [Display(Name = "Builty Date")] public DateTime BuiltyDate { get; set; }
        [Display(Name = "Builty NO.")] public string BuiltyNo { get; set; }
        [Display(Name = "Remarks")] [MaxLength(200)] public string Remarks { get; set; }
        public string Status { get; set; }
        [Display(Name = "Attachment")] [MaxLength(450)] public string Attachment { get; set; }
        [Display(Name = "Total Bonus")] [Column(TypeName = "numeric(18,2)")] public decimal TotalBonus { get; set; }
        [Display(Name = "Total D.C. Balance")] [Column(TypeName = "numeric(18,2)")] public decimal TotalDCBalance { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        [Display(Name = "Approved By")] [MaxLength(450)] public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsDeleted { get; set; }

        //navigation porperty
        public ARCustomer Customer { get; set; }
        public virtual ICollection<ARDeliveryChallanItem> ARDeliveryChallanItems { get; set; }
        [NotMapped]
        public bool Approve { get; set; }
        [NotMapped]
        public bool Unapprove { get; set; }

    }
}