using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class APOGPViewModel
    {
        public APOGP APOGP { get; set; }
        public List<APOGPDetails> APOGPDetails { get; set; }
        public string CreatedBy { get; set; }
        public string VendorName { get; set; }
        public List<string> UOMName { get; set; }
        public List<decimal> Rcd { get; set; }
        public List<decimal> Balc{get; set;}
        public List<string> Brand { get; set; } = new List<string>();
        public string OGPDate { get; set; }
    }
}
