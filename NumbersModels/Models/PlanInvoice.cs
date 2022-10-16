using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class PlanInvoice
    {
		[Key]
        public int Id { get; set; }
		[Display(Name ="Invoice #")]
        public int InvoiceNo { get; set; }
		[Display(Name ="Invoice Date")]
        public DateTime InvoiceDate { get; set; }
		[Display(Name = "Production Order #")]
		public int ProductionOrderId { get; set; }
		[Display(Name = "Vendor")]
		public int VendorId { get; set; }
		[MaxLength(450)]
        public string Remarks { get; set; }
		[MaxLength(450)]
        public string Attachment { get; set; }
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
		[MaxLength(15)]
		public string Status { get; set; }
	}
}