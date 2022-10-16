using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
	public class SaleReturnItems
	{
		public int Id { get; set; }
		public int SaleReturnId { get; set; }
		[ForeignKey("SecondLevel")]
		public int SecondItemCategory { get; set; }
		[ForeignKey("ThirdLevel")]
		public int ThirdItemCategory { get; set; }
		[ForeignKey("FourthLevel")]
		public int FourthItemCategory { get; set; }
		public int Meters { get; set; }
		public int MetersBalance { get; set; }
		public decimal Bales { get; set; }
		public decimal BalesBalance { get; set; }
		public InvItemCategories SecondLevel{get; set;}
		public InvItemCategories ThirdLevel { get; set;}
		public InvItemCategories FourthLevel { get; set;}
		public SaleReturn SaleReturn { get; set; }
	}
}