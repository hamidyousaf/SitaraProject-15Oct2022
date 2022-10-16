using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
     public class APPurchaseRequisitionDetails
    {
        [Key]
        public int Id { get; set; }
        public int APPurchaseRequisitionId { get; set; }
        public int ItemId { get; set; }
        public string Description { get; set; }
        public string ItemSpecification { get; set; }
        public string Code { get; set; }
        public DateTime RequiredDate { get; set; }
        public int UOM { get; set; }
        public string Attachment { get; set; }
        public string Brand { get; set; }
        public string TechanicalInfo { get; set; }
        [Column(TypeName = "numeric(18,2)")]
        public decimal Quantity { get; set; }
        public bool IsCSCreated { get; set; }
        public bool IsPOCreated { get; set; }

        public int CostCenterId { get; set; }
        public decimal CSRcd { get; set; }
        public decimal CSBalc { get; set; }
        public DateTime? LastPODate { get; set; }
        public decimal LastPOQty { get; set; }
        public decimal Consumption { get; set; }
        public CostCenter CostCenter { get; set; }
    }
}
