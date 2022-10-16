using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class InventoryViewModel
    {
        public InvItem InvItem { get; set; }
        public string UOM { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int TotalMeter { get; set; }
        public int BaleId { get; set; }
        public string Code { get; set; }
        public string LastPODate { get; set; }
        public decimal Comsumption { get; set; }
        public decimal LastPOQty { get; set; }
        public string BaleType { get; set; }
        public decimal Meters { get; set; }
        public int AvailableStock { get; set; }
        public int TempBales { get; set; }
        public decimal Rate { get; set; }
        [Column(TypeName = "numeric(18,6)")] public decimal AvgRate { get; set; }
        public decimal Quantity { get; set; }
        public string RequiredDate { get; set; }

    }
}
