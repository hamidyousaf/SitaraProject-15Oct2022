using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class VMGRGriegeRequisitionDetailsST
    {
        public int Id { get; set; }

        public int GriegeQualityId { get; set; }
        public int UOMId { get; set; }
        public int AvailableStock { get; set; }

        public int Qty { get; set; }

        public int seasonalFabricCons { get; set; }
        public int GRRequisitionIdST { get; set; }
        public string GRQuality { get; set; }
        public string UOMName { get; set; }

    }
}
