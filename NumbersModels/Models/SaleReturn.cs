using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class SaleReturn
    {
		public int Id { get; set; }
		[Display(Name ="Trans.#")]
		public int TransactionNo { get; set; }
		[Display(Name = "Date")]
		public DateTime SRDate { get; set; }
		[Display(Name = "Customer")]
		public int CustomerId { get; set; }
		[Display(Name = "IGP.#")]
		public int IGPId { get; set; }
		[Display(Name = "Builty #")]
		public string BuiltyNo { get; set; }
		[Display(Name = "Bales")]
		public decimal Bails { get; set; }

		public decimal BalanceQuantity { get; set; }
		public int TotalMeters { get; set; }
		public decimal TotalBales { get; set; }
		public string Status { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public int CompanyId { get; set; }
		public int Resp_ID { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public string ApprovedBy { get; set; }
		public DateTime ApprovedDate { get; set; }
		public string UnApprovedBy { get; set; }
		public DateTime UnApprovedDate { get; set; }
		public bool IsApproved { get; set; }
		//navigational property
		public ARInwardGatePass IGP { get; set; }
		public ARCustomer Customer { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
	}
}