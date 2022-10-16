using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
	public class ARSaleReturnInvoice
	{
		[Key]
		public int Id { get; set; }
		[Display(Name = "Invoice #")]
		public int InvoiceNo { get; set; }
		[Display(Name ="Invoice Date")]
		public DateTime InvoiceDate { get; set; }
		 
		[Display(Name ="Customer")]
		public int CustomerId { get; set; }
		[Display(Name ="Packing/Segregation #")]
		public int PackingId { get; set; }
		public int VoucherId { get; set; }
		public decimal TotalQty { get; set; }
		public decimal TotalDiscount { get; set; }
		public decimal TotalAmount { get; set; }
		 
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
		// navigational property

		public ARPacking Packing { get; set; }
		public ARCustomer Customer { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
	}
}