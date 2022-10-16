using System;
using System.Collections.Generic;
using System.Text;
using Numbers.Entity.Models;


namespace Numbers.Entity.ViewModels
{
    public class APIGPVm
    {
        public APIGP APIGP { get; set; }
        public List<APIGPDetails> APIGPDetails { get; set; }
        public string Vendor { get; set; }
        public string CreatedBy { get; set; }
        public List<string> UOMName { get; set; }
        public List< decimal> Balc { get; set; }
        public List<decimal> Rcd { get; set; }
        public List<decimal> TotalRecieved { get; set; } = new List<decimal>();
        public List<string> Brand { get; set; } = new List<string>();
        public  string IGPDate { get; set; }
        public string Category { get; set; }
        public int VoucherId { get; set; }
        public string Vehicle { get; set; }

    }
}
