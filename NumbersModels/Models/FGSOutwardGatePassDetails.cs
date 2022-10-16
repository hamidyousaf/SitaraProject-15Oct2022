using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
   public class FGSOutwardGatePassDetails
    {
        public int Id { get; set; }
        public int FGSOutwardGatePassId { get; set; }
        public int PONoId { get; set; }
        public int ItemId { get; set; }
        public string BaleType { get; set; }
        public int BaleId { get; set; }
        public int MeterBale { get; set; }
        public string BaleNo { get; set; }
        public int LotNo { get; set; }
        // navigational property
        public InvItem Item { get; set; }
        public BaleInformation Bale { get; set; }
    }
}
