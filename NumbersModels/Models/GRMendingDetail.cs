using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
   public class GRMendingDetail
    {
		public int Id { get; set; }
		public int GRMendingId { get; set; }
		public int IGPDetailId { get; set; }
		public int SrNo { get; set; }
		public decimal ReceivedQuantity { get; set; }
		public decimal RejectedQuantity { get; set; }
		public decimal FreshQuantity { get; set; }
		public decimal MendingQuantity { get; set; }
		public int DamageTypeId { get; set; }
	}
}