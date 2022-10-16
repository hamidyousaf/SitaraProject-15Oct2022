using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class InterSegmentalSaleTransferDetail
    {
        public int Id { get; set; }

        [ForeignKey("InterSegmentalSaleTransfer")]
        public int InterSegmentalSaleTransferId { get; set; }

        [Display(Name = "Item Category 2")]
        public int SecondItemCategoryId { get; set; }
        public int ItemId { get; set; }
        //public int UOMId { get; set; }
        public int BrandId { get; set; }
        [Display(Name = "Lot #")]
        public string LotNo { get; set; }
        public decimal WeaverQuantity { get; set; }
        public decimal MendingQuantity { get; set; }
        public decimal FoldingQuantity { get; set; }
        public decimal SaleTransferQuantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public InvItem Item { get; set; }
        public AppCompanyConfig UOM { get; set; }
    }
}
