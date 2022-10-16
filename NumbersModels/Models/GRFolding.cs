using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
   public class GRFolding
    {
		[Key]
		public int Id { get; set; }
		public int FoldingNo { get; set; }
		public DateTime FoldingDate { get; set; }
		[ForeignKey("Mending")]
		public int MendingId { get; set; }
		public int PurchaseContractNo { get; set; }
		public int WeavingContractNo { get; set; }
		public int PurchaseGRQualityId { get; set; }
		public int ContractGRQualityId { get; set; }
		public int MendingQty { get; set; }
		public int IGPLotNo { get; set; }		
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
		public decimal RecQty { get; set; }
		public decimal MendQty { get; set; }
		public decimal FoldQty { get; set; }
		public decimal GainLossQty { get; set; }
		public decimal ActQty { get; set; }
		public string Status { get; set; }

		//navigational property
		public GRMending Mending { get; set; }
		//public GRQuality PurchaseGRQuality { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
	}
}
