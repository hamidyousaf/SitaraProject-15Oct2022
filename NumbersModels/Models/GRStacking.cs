using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRStacking
    {

		[Key]
		public int Id { get; set; }
		[Display(Name = "Trans.#")]
		public int TransactionNo { get; set; }
		[Display(Name = "Trans. Date")]
		public DateTime TransactionDate { get; set; }
		[Display(Name = "GRN")]
		public int GRNId { get; set; }

		[Display(Name = "GRNNo")]
		public int GRNNo { get; set; }

		public string VendorName { get; set; }
		[Display(Name = "Vendor")]
		public int VendorId { get; set; }

		[Display(Name = "Weaving Contract")]
		public int WeavingContractId { get; set; }
		[Display(Name = "Purchase Contract")]
		public int PurchaseContractId { get; set; }
		 
		[Display(Name = "folded Pieces")]
		public decimal FoldedPieces { get; set; }
		[Display(Name = "Folded Quantity")]
		public decimal FoldedQuantity { get; set; }
		[Display(Name = "Contract Greige Quality")]
		public string GreigeContractQuality { get; set; }
		[Display(Name = "On Loom Greige Quality")]
		public string GreigeContractQualityLoom { get; set; }
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





	}
}
