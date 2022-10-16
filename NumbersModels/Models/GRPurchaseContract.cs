using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRPurchaseContract
    {
		[Key]
		public int Id { get; set; }
		public int ContractNo { get; set; }
		public DateTime ContractDate { get; set; }
		public DateTime DeliveryDate { get; set; }
		[ForeignKey("Vendor")]
		public int VendorId { get; set; }
		public int PurchaseGRQualityId { get; set; }
		public int ContractGRQualityId { get; set; }
		public int ContractQuantity { get; set; }
		public int BalanceContractQty { get; set; }
		public decimal RatePerMeter { get; set; }
		public decimal ExSalesTaxAmount { get; set; }
		public int SalesTaxId { get; set; }
		public decimal SalesTaxAmount { get; set; }
		public decimal InSalesTaxAmount { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public int CompanyId { get; set; }
		public int Resp_ID { get; set; }
		[MaxLength(450)]
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }
		[MaxLength(450)]
		public string UpdatedBy { get; set; }
		public string ApprovedBy { get; set; }
		public DateTime ApprovedDate { get; set; }
		[MaxLength(450)]
		public string UnApprovedBy { get; set; }
		public DateTime UnApprovedDate { get; set; }
		public bool IsApproved { get; set; }
		public string Status { get; set; }
		//navigational property
		
		public APSupplier Vendor { get; set; }
		public GRQuality PurchaseGRQuality { get; set; }
		public GRQuality ContractGRQuality { get; set; }

		[NotMapped]
		public GRPurchaseContract purchaseContract { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
        public int GRRequisitionId  { get; set; }
    }
}