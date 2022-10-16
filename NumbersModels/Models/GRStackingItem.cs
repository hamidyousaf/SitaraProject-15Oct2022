using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRStackingItem
	{
		[Key]
		public int Id { get; set; }
		public int GRGRNId { get; set; }
		[ForeignKey("WareHouse")]
		public int WareHouseId { get; set; }
		[ForeignKey("Location")]
		public int LocationId { get; set; }
		public decimal Quantity { get; set; }
		public decimal BalQty { get; set; }
		public decimal Rate { get; set; }
		public decimal Amount { get; set; }

	public AppCompanyConfig WareHouse { get; set; }
	public AppCompanyConfig Location { get; set; }


	}
}
