using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
	public class ARSaleReturnInvoiceItems
	{
		public int Id { get; set; }
		[ForeignKey("Packing")]
		public int PackingId { get; set; }
		[ForeignKey("FourthLevel")]
		public int FourthItemCategoryId { get; set; }
		[ForeignKey("Item")]
		public int ItemId { get; set; }
		[ForeignKey("SaleInvoice")]
		public int SaleInvoiceId { get; set; }
		public int SRInvoiceId { get; set; }
		public int SaleInvoiceItemId { get; set; }
		public decimal Qty { get; set; }
		public decimal Rate { get; set; }
		public decimal Discount { get; set; }
		public decimal Amount { get; set; }
		[ForeignKey("ReasonType")]
		public int ReasonTypeId { get; set; }
		[ForeignKey("ReturnType")]
		public int ReturnTypeId { get; set; }
		[ForeignKey("Season")]
		public int SeasonId { get; set; }
		public InvItemCategories FourthLevel { get; set;}
		public InvItem Item { get; set;}
		public ARPacking Packing { get; set;}
		public AppCompanyConfig ReasonType { get; set;}
		public AppCompanyConfig ReturnType { get; set;}
		public AppCompanyConfig Season { get; set;}
		public ARInvoice SaleInvoice { get; set;}
		public ARSaleReturnInvoice SRInvoice { get; set;}
	}
}