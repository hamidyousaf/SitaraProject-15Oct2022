using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
	public  class SeasonalPlaning
	{
		[Key]
		public int Id { get; set; }
		[Display(Name ="Trans.#")]
		public int TransactionNo { get; set; }
		[Display(Name = "Trans. Date")]
		public DateTime TransactionDate { get; set; }
		[Display(Name = "Season")]
		public int SeasonId { get; set; }
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
		public AppCompanyConfig Season { get; set; }
		public virtual ICollection<SeasonalPlaningDetail> SeasonalPlaningDetail { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
	}
}
