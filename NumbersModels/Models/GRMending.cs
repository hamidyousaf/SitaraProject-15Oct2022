using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRMending
    {
		[Key]
		public int Id { get; set; }
		[Display(Name ="Trans.#")]
		public int TransactionNo { get; set; }
		[Display(Name = "Trans. Date")]
		public DateTime TransactionDate { get; set; }
		[Display(Name = "IGP #")]
		public int GRIGPId { get; set; }
		public int WeavingContractNo { get; set; }
		public int PurchaseContractNo { get; set; }
		public int ContractQualityId { get; set; }
		public int LoomQualityId { get; set; }
		public decimal ReceivedQuantity { get; set; }
		public string LotNo { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public int CompanyId { get; set; }
		public int Resp_Id { get; set; }
		[MaxLength(450)]
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		[MaxLength(450)]
		public string UpdatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
		public bool IsApproved { get; set; }
		[MaxLength(450)]
		public string ApprovedBy { get; set; }
		public DateTime ApprovedDate { get; set; }
		[MaxLength(450)]
		public string UnApprovedBy { get; set; }
		public DateTime UnApprovedDate { get; set; }
		public string Status { get; set; }
		// navigational property
		public GRInwardGatePass GRIGP { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
		public decimal TotalRecievedQuantity { get; set; }
		public decimal TotalRejectedQuantity { get; set; }
		public decimal TotalFreshQuantity { get; set; }
		public decimal TotalGradedQuantity { get; set; }
	}
}