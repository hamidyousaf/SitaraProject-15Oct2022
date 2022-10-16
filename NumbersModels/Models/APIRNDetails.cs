using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class APIRNDetails
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("IRN")]
        public int IRNID { get; set; }
        [ForeignKey("Item")]
        public int ItemID { get; set; }
        public int BrandId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDiscription { get; set; }
        public string UOM { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        public decimal IGP_Qty { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        public decimal Received_Qty { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        public decimal Accepted_Qty { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        public decimal Rejected_Qty { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        
        public decimal GRNQty { get; set; }
        public decimal OGPBalQty { get; set; }
        public decimal OGPQty { get; set; }
        public bool IsOGPCreated { get; set; }
        public string Reason_OF_Rejection { get; set; }
        public int PrID { get; set; }
        public int IGPDetailId { get; set; }
        public int PrDetailId { get; set; }
        public int CategoryId { get; set; }
        public InvItem Item { get; set; }
        [NotMapped]
        public string CategoryName { get; set; }
        public InvItemCategories Category { get; set; }
        public APIRN IRN { get; set; }
        public AppCompanyConfig Brand { get; set; }
    }
}