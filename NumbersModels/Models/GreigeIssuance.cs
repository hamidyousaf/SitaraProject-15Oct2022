using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
	public  class GreigeIssuance
	{
		[Key]
		public int Id { get; set; }
		[Display(Name ="Trans.#")]
		public int TransactionNo { get; set; }
		[Display(Name = "Trans. Date")]
		public DateTime TransactionDate { get; set; }
		[Display(Name = "Specification")]
		[ForeignKey("Specification")]
		public int SpecificationId { get; set; }
        [ForeignKey("Vendor")]
		public int VendorId { get; set; }
		[ForeignKey("IssueType")]
		public int IssueTypeId { get; set; }
		[ForeignKey("WareHouse")]
		public int WareHouseId { get; set; }
		public int VoucherId { get; set; }
		public string process { get; set; }
		public int productionId { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? UpdatedDate { get; set; }
		[MaxLength(450)]
		public string? UpdatedBy { get; set; }
		[MaxLength(450)]
		public string? ApprovedBy { get; set; }
		public DateTime? ApprovedDate { get; set; }
		[MaxLength(450)]
		public string? UnApprovedBy { get; set; }
		public DateTime? UnApprovedDate { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public int CompanyId { get; set; }
		public int Resp_Id { get; set; }
		public bool IsApproved { get; set; }
		public AppCompanyConfig IssueType { get; set; }
		public AppCompanyConfig WareHouse { get; set; }
		//public PlanSpecification Specification { get; set; }
        public APSupplier Vendor { get; set; }
        public ProductionOrder Specification { get; set; }

		public virtual ICollection<GreigeIssuanceDetail> GreigeIssuanceDetail { get; set; }
	}
}
