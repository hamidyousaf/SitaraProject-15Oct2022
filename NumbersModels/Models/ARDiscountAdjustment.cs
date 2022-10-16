using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
	public class ARDiscountAdjustment
	{
		[Key]
		public int Id { get; set; }
		[Display(Name = "Trans.#")]
		public int TransactionNo { get; set; }
		[Display(Name = "Product Type")]
		public int ProductType_Id { get; set; }
		[Display(Name = "Item Category")]
		public int ItemCategory_Id { get; set; }
		[Display(Name = "Third Level")]
		public int ThirdLevel_Id { get; set; }
		[Display(Name = "Brand")]
		public int Brand_Id { get; set; }
		public int CityId { get; set; }
		public int CustomerDiscountId { get; set; }
		[Display(Name = "Customer")]
		public int Customer_Id { get; set; }
		[Display(Name ="Start Date")]
		public DateTime StartDate { get; set; }
		[Display(Name ="End Date")]
		public DateTime EndDate { get; set; }
		[Display(Name = "Discount %")] [Column(TypeName = "numeric(18,4)")] public decimal DiscountPercent { get; set; }
		public string ApprovedBy { get; set; }
		public DateTime ApprovedDate { get; set; }
		public string UnApprovedBy { get; set; }
		public DateTime UnApprovedDate { get; set; }
		public bool IsApproved { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public int CompanyId { get; set; }
		[Display(Name = "Sales Person")]
		public int SalesPerson_Id { get; set; }
		[Display(Name = "Commission Agent")]
		public int Agent_Id { get; set; }
		public int Resp_Id { get; set; }
		public string Status { get; set; }
		public int VoucherId { get; set; }
		[Display(Name = "Grand Total")]
		[Column(TypeName = "numeric(18,4)")]
		public decimal GrandTotal { get; set; }
		public bool? IsClosed { get; set; }
		public ARCustomer Customer_ { get; set; }
		public ARDiscount CustomerDiscount { get; set; }
        public virtual ICollection<ARDiscountAdjustmentItem>  DiscountItems{ get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
	}
}