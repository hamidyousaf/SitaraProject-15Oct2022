using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARCreditLimit
    {
		[Key]
		public int Id { get; set; }
		public int? ARCustomerId { get; set; }
		public DateTime FromDate { get; set; }
		public DateTime ToDate { get; set; }
		public string CreditLimit { get; set; }
		public int? CompanyId { get; set; }
		public bool IsActive { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public string UpdatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsClosed { get; set; }
		public bool? IsCreditLimitExceed { get; set; }

		[NotMapped]
		public decimal CreditLimitList { get; set; }
		[NotMapped]
		public decimal ledgerDebit { get; set; }
		[NotMapped]
		public decimal ledgerCredit { get; set; }
		[NotMapped]
		public decimal ledgerbalance { get; set; }
		public bool IsExpired { get; set; }
		public decimal RemainingBalance { get; set; }
	}
}