using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class GRGRNViewModel
    {
        public GRGRN GRGRN { get; set; } = new GRGRN();
        public string TransactionDate { get; set; }
        public string VendorName { get; set; }
        public int Id { get; set; }
        public int TransactionNo { get; set; }
        public int FoldingNo { get; set; }
        public int PurcahseContracNo { get; set; }
        public int WeavingContactNo { get; set; }
        public SelectList FeederLov { get; set; }
        public SelectList WeavingContractLOV { get; set; }
        public SelectList PurchaseContractLOV { get; set; }

        public SelectList WareHouse { get; set; }
        public SelectList Location { get; set; }
        public SelectList Penalty { get; set; }
        public GRGRNItem[] GRGRNItem { get; set; }
        public GRStackingItem[] GRStackingItem { get; set; }
		public decimal Quantity { get; set; }
        public decimal RateOfConversionIncTax { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPenaltyAmount { get; set; }
        public decimal LessYarnPrice { get; set; }
        public decimal NetPenaltyAmount { get; set; }
        public decimal NetPayableAmount { get; set; }
        public GRWeavingContract GRWeavingContract { get; set; }
        public GRPurchaseContract GRPurchaseContract { get; set; }
        public InvItem Item { get; set; }
    }
}
