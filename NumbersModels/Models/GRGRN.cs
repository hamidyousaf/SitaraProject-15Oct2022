using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRGRN
	{
		[Key]
        public int Id { get; set; }
		[Display(Name = "Trans.#")]
		public int TransactionNo { get; set; }
		[Display(Name = "Trans. Date")]
		public DateTime TransactionDate { get; set; }
		
		[Display(Name = "Folding #")]
		[ForeignKey("Folding")]
		public int FoldingId { get; set; }
		[Display(Name = "Weaving Contract #")]
		//[ForeignKey("WeavingContract")]
		public int WeavingContractId { get; set; }
		[Display(Name = "Purchase Contract #")]
		//[ForeignKey("PurchaseContract")]
		public int PurchaseContractId { get; set; }
		[Display(Name ="Vendor Name")]
		[ForeignKey("Vendor")]
		public int VendorId { get; set; }
		[Display(Name = "Contract Rate")]
		public decimal ContractRate { get; set; }
		[Display(Name = "Contract Greige Quality")]
		public string GreigeContractQuality { get; set; }
		[Display(Name = "On Loom Greige Quality")]
		public string GreigeContractQualityLoom { get; set; }
		public string VendorName { get; set; }
		public string End { get; set; }
		public string Picks { get; set; }
		public string Width { get; set; }
		public decimal WovenPieces { get; set; }
		public decimal WovenQty { get; set; }
		public decimal MendedPieces { get; set; }
		public decimal MendedQty { get; set; }
		public decimal FoldedPieces { get; set; }
		public decimal FoldedQty { get; set; }
		public decimal RejectedQty { get; set; }
		public decimal BalanceQuantity { get; set; }
		public decimal TotalQuantity { get; set; }
		public decimal TotalAmount { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public int VoucherId { get; set; }
		public int CompanyId { get; set; }
		public int Resp_Id { get; set; }
		[MaxLength(450)]
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }
		[MaxLength(450)]
		public string UpdatedBy { get; set; }
		[MaxLength(450)]
		public string ApprovedBy { get; set; }
		public bool IsApproved { get; set; }
		public DateTime ApprovedDate { get; set; }
		[MaxLength(450)]
		public string UnApprovedBy { get; set; }
		public DateTime UnApprovedDate { get; set; }
		public string Status { get; set; }

		public APSupplier Vendor { get; set; }
		public GRFolding Folding { get; set; }
		public GRWeavingContract WeavingContract { get; set; }
		public GRPurchaseContract PurchaseContract { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
	}
}