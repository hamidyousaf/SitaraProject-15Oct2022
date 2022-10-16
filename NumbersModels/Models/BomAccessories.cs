using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
   public class BomAccessories
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
        public decimal Kameez { get; set; }
		public decimal Shalwar { get; set; }
		public decimal Dupatta { get; set; }

		[Display(Name = "Mtr / Item")]
		public decimal MeterItem { get; set; }

		[Display(Name = "Item / Bolt")]
		public decimal BoltItem { get; set; }

		[Display(Name = "Item / Dzn")]
		public decimal DozenItem { get; set; }

		[Display(Name = "Mtr / Dzn")]
		public decimal MeterDozen { get; set; }

		[Display(Name = "Mtr / Bolt")]
		public decimal MeterBolt { get; set; }

		[Display(Name = "Item / Bale")]
		public decimal BaleItem { get; set; }

		[Display(Name = "Dzn / Bale")]
		public decimal DozenBale { get; set; }

		[Display(Name = "Bolt / Bale")]
		public decimal BoltBale { get; set; }

		[Display(Name = "Bale Mtr")]
		public decimal BaleMeter { get; set; }

		[Display(Name = "Carton Type")]
		public int CartonTypeId { get; set; }

		[Display(Name = "SP / Carton")]
		public decimal SpCarton { get; set; }

		[Display(Name = "Carton Mtr")]
		public decimal CartonMeter { get; set; }
		public string Remarks { get; set; }
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
		public int ItemType { get; set; }
		// navigational property
		public InvItemCategories SecondItemCategory { get; set; }
		public InvItemCategories FourthItemCategory { get; set; }
		public AppCompanyConfig CartonType { get; set; }
		[NotMapped]
		public bool Approve { get; set; }
		[NotMapped]
		public bool Unapprove { get; set; }
	}
}
