using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRGriegeRequisitionDetailsST
    {
        public int Id { get; set; }
   
        public int GriegeQualityId { get; set; }
        public int UOMId { get; set; }
        public int AvailableStock { get; set; }
       
        public int Qty { get; set; }
      
        public int GRRequisitionIdST { get; set; }
     
        public GRQuality GriegeQuality { get; set; }
    }
}
