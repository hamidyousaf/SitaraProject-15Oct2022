using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class InterSegmentalSaleTransfer
    {
        public int Id { get; set; }
        [Display(Name = "Trans #")]
        public int TransactionNo { get; set; }
        [Display(Name = "Trans Date")]
        public DateTime TransactionDate { get; set; }
		[Display(Name = "Trans Type")]
		public int TransTypeId { get; set; }

		[Display(Name = "Segment / Customer")]
		public int SegmentCustomerId { get; set; }
		public string Remarks { get; set; }
		[Display(Name = "GRN #")]
		public int GRNId { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string ApprovedBy { get; set; }
		public DateTime ApprovedDate { get; set; }
		public string UnApprovedBy { get; set; }
		public DateTime UnApprovedDate { get; set; }
		public string Status { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public int CompanyId { get; set; }
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
	}
}
