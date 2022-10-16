using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRInwardGatePass
    {
		[Key]
        public int Id { get; set; }
		[Display(Name = "Trans.#")]
		public int TransactionNo { get; set; }
		[Display(Name = "Trans. Date")]
		public DateTime TransactionDate { get; set; }
		[Display(Name = "Weaving Contract")]
		public int WeavingContractId { get; set; }
		[Display(Name = "Purchase Contract")]
		public int PurchaseContractId { get; set; }
		[Display(Name ="Lot #")]
		public string LotNo { get; set; }
		[Display(Name = "Quantity in Meters")]
		public decimal Quantity { get; set; }
		[Display(Name = "Balance in Meters")]
		public decimal BalanceQuantity { get; set; }
		public decimal TotalRecievedQuantity { get; set; }
		public decimal TotalMeasureQuantity { get; set; }
		public decimal TotalActualQuantity { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
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
		//public int GreigeQualityLoomId { get; set; }
		//public int ContractGRQualityId { get; set; }
		
		// navigational property
		public GRWeavingContract WeavingContract { get; set; }
		public GRPurchaseContract PurchaseContract { get; set; }
		//public GRPurchaseContract ContractGRQuality { get; set ;}

		//public GRQuality GreigeQuality { get; set; }
		//public GRQuality GreigeQualityLoom { get; set; }

		//public GRQuality ContractGRQuality { get; set; }

		//public GRInwardGatePass gRInwardGatePass { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
	}
}