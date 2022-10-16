using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APShipmentDetailList
    {
        public int ID { get; set; }
        public int ShipmentNo { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string Vendor { get; set; }
        public string ShipmentAgent { get; set; }
        public string CreatedBy { get; set; }
        public string UserName { get; set; }
    }
}
