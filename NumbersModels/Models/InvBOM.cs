using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
  public  class InvBOM
    {
        public int Id { get; set; }
		[Display(Name = "Trans #")]
		public int TransactionNo { get; set; }
		[Display(Name = "Trans Date")]
		public DateTime TransactionDate { get; set; }
		[Display(Name = "Item Category 2")]
		public int SecondItemCategoryId { get; set; }
		
		[Display(Name = "Item Category 4")]
		public int FourthItemCategoryId { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string ApprovedBy { get; set; }
		public DateTime ApprovedDate { get; set; }
		public string UnApprovedBy { get; set; }
		public string Status { get; set; }
		public DateTime UnApprovedDate { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public int CompanyId { get; set; }
      // navigational property
		public InvItemCategories SecondItemCategory { get; set; }
		public InvItemCategories FourthItemCategory { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
	}
}
