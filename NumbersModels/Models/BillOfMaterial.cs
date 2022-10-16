using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class BillOfMaterial
    {
        [Key]
        public int Id { get; set; }
        public int ProductionOrderId { get; set; }
        public int FourthItemCategoryId { get; set; }
        public int TotalQuantity { get; set; }
        public int BoltQuantity { get; set; }
        public int SuitePieceQuantity { get; set; }
    }
}