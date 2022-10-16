using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARRecoveryPercentage
    {
        public int Id { get; set; }
		[Display(Name = "Item Category")]
		public int ItemCategory_Id { get; set; }
		[Display(Name ="Trans. #")]
        public int TransactionNo { get; set; }
		public string ApprovedBy { get; set; }
		public DateTime ApprovedDate { get; set; }
		public string UnApprovedBy { get; set; }
		public DateTime UnApprovedDate { get; set; }
		public bool IsApproved { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public int? CompanyId { get; set; }
		public int ResponsibilityId { get; set; }
		public string Status { get; set; }

		public virtual ICollection<ARRecoveryPercentageItem> ARRecoveryPercentageItems { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
	}
}
