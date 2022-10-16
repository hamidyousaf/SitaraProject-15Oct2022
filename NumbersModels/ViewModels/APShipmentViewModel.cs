using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class APShipmentViewModel
    {
        public APShipment APShipment { get; set; }
        public APCustomInfo APCustomInfo { get; set; }
        public List<APCustomInfoDetails> APCustomInfoDetails { get; set; }
        public List<APShipmentDetail> APShipmentDetails { get; set; }
        public List<ShipmentVM> shipmentVMs { get; set; }
        public string UserName { get; set; }
        public string VendorName { get; set; }
        public string ShipDate { get; set; }
        public string Agent { get; set; }
        public int POId { get; set; }
        public int PONo { get; set; }

    }
}
