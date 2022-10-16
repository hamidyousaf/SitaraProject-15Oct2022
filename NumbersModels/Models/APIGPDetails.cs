using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
   public class APIGPDetails
    {
        [Key]
        public int Id { get; set; }
        public int IGPId { get; set; }
        [ForeignKey("APPurchaseOrder")]
        public int PoNo { get; set; }
        public int PoId { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDiscription { get; set; }
        public string UOM { get; set; }
        public DateTime PoDlvDate { get; set; }
        [Column(TypeName = "numeric(18,2)")]
        public decimal PoQty { get; set; }
        [Column(TypeName = "numeric(18,2)")]
        public decimal RCDQty { get; set; }
        [Column(TypeName = "numeric(18,2)")]
        public decimal BalQty { get; set; }
        [Column(TypeName = "numeric(18,2)")]
        public decimal IGPQty { get; set; }
        public int PoDetailId { get; set; }
        public int PrDetailId { get; set; }
        public bool IsIRNCreated { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public int ShipmentDetailId { get; set; }
        public decimal Packages { get; set; }
        public decimal PackagesQty { get; set; }
        [NotMapped]
        public string CategoryName { get; set; }
        //Navigation Property
     
        public InvItemCategories Category { get; set; }
        public APPurchaseOrder APPurchaseOrder { get; set; }
    }
}
