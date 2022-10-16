using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRFoldingItems
    {
		[Key]
		public int Id { get; set; }
		public int FoldingId { get; set; }	
		public decimal ReceivedQty { get; set; }
		public decimal MendQty { get; set; }
		public decimal FoldQty { get; set; }
		public decimal GainLossQty { get; set; }
		public decimal ActualFoldQty { get; set; }
		public int SrNo { get; set; }

	}
}
