using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRGriegeRequisitionDetails
    {
        public int Id { get; set; }
        public int SPNo { get; set; }
        public int GriegeQualityId { get; set; }
        public int UOMId { get; set; }
        public int AvailableStock { get; set; }
        public int ReserveAvailableQty { get; set; }
        public int Qty { get; set; }
        public int BalanceQty { get; set; }
        public decimal WarpBag { get; set; }
        public decimal WeftBag { get; set; }
        public string WarpBagWOQ { get; set; }
        public string WeftBagWOQ { get; set; }
        public int GRRequisitionId { get; set; }
        public bool IsUsed { get; set; }
        //navigational property
        public GRQuality GriegeQuality { get; set; }
    }
}
