using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
   public class FGSOutwardGatePassViewModel
    {
        public FGSOutwardGatePass FGSOutwardGatePass { get; set; } = new FGSOutwardGatePass();
        public FGSOutwardGatePassDetails[] FGSOutwardGatePassDetails { get; set; }
        public SelectList WareHouseLOV { get; set; }
        public SelectList Customer { get; set; }
        public SelectList SecondLevelCategoryLOV { get; set; }
        public SelectList FourthLevelCategoryLOV { get; set; }
        //public string Date { get; set; }

        public string Date { get; set; }
    }
}
