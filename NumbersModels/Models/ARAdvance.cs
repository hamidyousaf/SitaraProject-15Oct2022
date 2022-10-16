using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARAdvance
    {
		public int Id { get; set; }
		public int TransactionNo { get; set; }
		public DateTime TransactionDate { get; set; }
		[ForeignKey("Customer")]
		public int CustomerId { get; set; }
		[ForeignKey("AdvanceAccount")]
		public int AdvanceAccountId { get; set; }
		[ForeignKey("GLBankCash")]
		public int GLBankCashId { get; set; }
		public string RefrenceNo { get; set; }
		public DateTime ReferanceDate { get; set; } 
		public decimal Amount { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public int CompanyId { get; set; }
		public int Resp_Id { get; set; }
		[MaxLength(450)]
		public string CreatedBy { get; set; }

		public DateTime CreatedDate { get; set; }
		[MaxLength(450)]
		public string UpdatedBy { get; set; }

		public DateTime? UpdatedDate { get; set; }

		public string ApprovedBy { get; set; }
		public DateTime ApprovedDate { get; set; }
		 public bool IsApproved { get; set; }
		 public string Status { get; set; }
		 public string Remarks { get; set; }
		public int VoucherId { get; set; }
		public ARCustomer Customer { get; set; }
		public GLAccount AdvanceAccount { get; set; }
		public GLBankCashAccount GLBankCash { get; set; }

		[NotMapped]
		public SelectList dtoCustomer { get; set; }
		[NotMapped]
		public SelectList dtoAdvanceAccount { get; set; }
		[NotMapped]
		public SelectList dtoGLBankCash { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
		[NotMapped]
		public string Date { get; set; }
		[NotMapped]
		public ARAdvance ARAdvanceModel { get; set; }
	}
}