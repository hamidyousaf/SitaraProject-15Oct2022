using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ProductionOrder
    {
		[Key]
		public int Id { get; set; }
		[Display(Name ="Production Order#")]
		public string TransactionNo { get; set; }
		[Display(Name = "Transaction Date")]
		public DateTime TransactionDate { get; set; }
		[Display(Name = "MP #")]
		public int MonthlyPlanningId { get; set; }
		[Display(Name = "Item Category 2")]
		[NotMapped]
		public int SecondItemCategoryId { get; set; }
		[Display(Name = "Process")]
		public int ProcessId { get; set; }
		[Display(Name = "Vendor")]
		public int VendorId { get; set; }
		[Display(Name = "Monthly Qty (mtr)")]
		public int MonthlyQuantity { get; set; }
		public string PlanOf { get; set; }
		[MaxLength(50)]
		public string Status { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public int CompanyId { get; set; }
		public int TransferToCompany { get; set; }
		public int Resp_Id { get; set; }
		public int TotalQty { get; set; }
		[MaxLength(450)]
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
		[MaxLength(450)]
		public string UpdatedBy { get; set; }
		[MaxLength(450)]
		public string ApprovedBy { get; set; }
		public DateTime? ApprovedDate { get; set; }
		[MaxLength(450)]
		public string UnApprovedBy { get; set; }
		public DateTime? UnApprovedDate { get; set; }
		public bool? IsApproved { get; set; }
		//navigational property
		public PlanMonthlyPlanning MonthlyPlanning { get; set; }
		public AppCompanyConfig Process { get; set; }
		//public InvItemCategories SecondItemCategory { get; set; }
		public ICollection<ProductionOrderItem> ProductionOrderItems { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
		public APSupplier Vendor { get; set; }
	}
}