using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GreigeIssuanceDetail
    {
        [Key]
        public int Id { get; set; }
        //public int SeasonalPlaningId { get; set; }
        public int GreigeIssuanceId { get; set; }
        public int ProductionOrderDetailId { get; set; }
        public int ProductionId { get; set; }


        [Display(Name = "Greige Quality ")]
        public int GreigeQualityId { get; set; }
        public decimal RequiredQty { get; set; }
        public decimal AvailableQty { get; set; }
        public decimal IssuanceQty { get; set; }
    

        //navigational property
        public GRQuality GreigeQuality { get; set; }

        public GreigeIssuance GreigeIssuance { get; set; }
        public ProductionOrderItem ProductionOrderDetail { get; set; }
        //public BillOfMaterials BillOfMaterials { get; set;}

        [NotMapped]
        public decimal AlreadyIssuedQty { get; set; }
    }

}
