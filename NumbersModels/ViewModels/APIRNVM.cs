using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class APIRNVM
    {
        public APIRN APIRN { get; set; }
        public List<APIRNDetails> APIRNDetails { get; set; }
        public string CreatedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string VendorName { get; set; }
        public List<string> UOMName { get; set; }
        public List<string> Brand { get; set; } = new List<string>();
        public List<decimal> Rcd { get; set; }
        public List<decimal> Balc{get; set;}
        public string IRNDate { get; set; }
    }
}
