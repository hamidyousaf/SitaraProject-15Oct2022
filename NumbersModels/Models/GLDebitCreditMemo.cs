using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GLDebitCreditMemo
    {
        public int Id { get; set; }
		[Display(Name = "Trans.#")]
        public int TransactionNo { get; set; }
		[Display(Name = "Trans. Date")]
        public DateTime TransactionDate { get; set; }
		[Display(Name = "Trans. Type")]
		public int TransactionTypeId { get; set; }
		[Display(Name = "Document #")]
		public string DocumentNo { get; set; }
		[Display(Name = "Operation Unit")]
		public int OperatingUnitId { get; set; }
		[Display(Name = "Party")]
		public int PartyId { get; set; }
		public int VoucherId { get; set; }
		public string PartyType { get; set; }
		[Display(Name = "Reference #")]
        public string ReferenceNo { get; set; }
		[Display(Name = "Document Date")]
		public DateTime DocumentDate { get; set; }
		public string Remarks { get; set; }
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
		public decimal Total { get; set; }
		[Display(Name = "Discount Amount")]
		public decimal TotalDiscountAmount { get; set; }
		[Display(Name = "ST Amount")]
		public decimal TotalSTAmount { get; set; }
		[Display(Name = "Grand Total")]
		public decimal GrandTotal { get; set; }
		//navigational property
		public AppCompanyConfig TransactionType { get; set; }
	}
}