using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class VMGRGriegeRequisitionDetails
    {
        public int Id { get; set; }
        public int SPNo { get; set; }
        public int GriegeQualityId { get; set; }
        public int UOMId { get; set; }
        public int AvailableStock { get; set; }
        public int ReserveAvailableQty { get; set; }
        public int Qty { get; set; }
        public decimal WarpBag { get; set; }
        public decimal WeftBag { get; set; }
        public string WarpBagWOQ { get; set; }
        public string WeftBagWOQ { get; set; }
        public int seasonalFabricCons { get; set; }
        public int GRRequisitionId { get; set; }
        public string GRQuality { get; set; }
        public string UOMName { get; set; }
       
    }
}
