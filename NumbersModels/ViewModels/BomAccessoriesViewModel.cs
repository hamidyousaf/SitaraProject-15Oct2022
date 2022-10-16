using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class BomAccessoriesViewModel
    {
        public BomAccessories BomAccessories { get; set; } = new BomAccessories();
        public List<BomAccessoriesDetail> BomAccessoriesDetail { get; set; }
        public string Date { get; set; }
        public string FourthCatText { get; set; }
        public string SecondCatText { get; set; }

    }
}
