using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class FGSInwardGatePassDetail
	{
		[Key]
		public int Id { get; set; }
		public int FGSInwardGatePassId { get; set; }
		public int ProductionOrderId { get; set; }
		public int ItemId { get; set; }
		public int BaleId { get; set; }
		public int FGSOutwardGatePassId { get; set; }
		public int FGSOutwardGatePassDetailId { get; set; }
		public int MeterPerBales { get; set; }
		public string BaleType { get; set; }
		[MaxLength(50)]
		public string BaleNo { get; set; }
		[MaxLength(50)]
		public string LotNo { get; set; }
		public InvItem Item { get; set; }
	}
}