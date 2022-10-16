using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRInwardGatePassDetail
    {
		[Key]
		public int Id { get; set; }
		public int GRIGPId { get; set; }
		public int SrNo { get; set; }
		public decimal ReceivedQuantity { get; set; }
		public decimal MeasureQuantity { get; set; }
		public decimal ActualQuantity { get; set; }
		//public GRInwardGatePass GRInwardGate { get; set; }
	}
}