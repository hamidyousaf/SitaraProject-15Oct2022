using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APShipmentDetail
    {
        [Key]
        public int Id { get; set; }
        public int APShipmentId { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public int UOM { get; set; }
        public int ItemId { get; set; }
        public int Category { get; set; }
        public int Origin { get; set; }
        public string HSCode { get; set; }
        public decimal ShippedQty { get; set; }
        public decimal Rate { get; set; }
        public decimal FCValue { get; set; }
        public decimal PkrValue { get; set; }
        public string Remarks { get; set; }
        public decimal POQty { get; set; }
        public bool IsIGP { get; set; }
        public int PrDetailId {get;set;}
        public int PoDetailId { get;set;}
        [NotMapped]
        public string Date { get; set; }
    }
}