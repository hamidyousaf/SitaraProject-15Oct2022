using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class GRStackingViewModel
    {
        public GRStacking GRStacking { get; set; } = new GRStacking();
        public string TransactionDate { get; set; }
        public string VendorName { get; set; }
        public int Id { get; set; }
        public int TransactionNo { get; set; }
        public int GRNNo { get; set; }
        public int PurcahseContracNo { get; set; }
        public int WeavingContactNo { get; set; }
        public SelectList GRNLov { get; set; }
        public SelectList VendorLov { get; set; }
        public SelectList WeavingContractLOV { get; set; }
        public SelectList PurchaseContractLOV { get; set; }
        public SelectList WareHouse { get; set; }
        public SelectList Location { get; set; }
        public GRStackingItem[] GRStackingItem { get; set; }


    }
}
