using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRGRNItem
	{
		[Key]
		public int Id { get; set; }
		public int GRGRNId { get; set; }
		[ForeignKey("Penalty")]
		public int PenaltyId { get; set; }
		public decimal Sample { get; set; }
		public decimal ActualWidth { get; set; }
		public decimal ActualPick { get; set; }
		public decimal PenaltyRate { get; set; }
		public decimal Quantity { get; set; }
		public decimal Amount { get; set; }

		public AppCompanyConfig Penalty { get; set; }
	}
}