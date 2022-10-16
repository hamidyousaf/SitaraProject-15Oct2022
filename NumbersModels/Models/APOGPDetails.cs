using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class APOGPDetails
    {
        [Key]
        public int Id { get; set; }
        public int OGPId { get; set; }
        public int PrID { get; set; }
        public int ItemId { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        public decimal IRNRejectedQty { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        public decimal ReceivedQty { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        public decimal BalanceQty { get; set; }
        public int IRNId { get; set; }
        public int BrandId { get; set; }
        public int IRNDetailId { get; set; }
        public int PrDetailId { get; set; }
        //navigational property
        public InvItem Item { get; set; }
        public APIRN IRN { get; set; }
    }
}