using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APCSRequest
    {
        public int ID { get; set; }
        public int APComparativeStatement_ID { get; set; }
        public int PR { get; set; }
        public string ItemDescription { get; set; }
        public string UOM { get; set; }
        public Decimal Indent_Quantity { get; set; }
        public Decimal Total_App_Amount { get; set; }
        public DateTime LastPO { get; set; }
        public Decimal LastPORate { get; set; }
        public string Request { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public int PrDetailId { get; set; }
        public bool IsPOApproved { get; set; }
        public DateTime LastPODate { get; set; }
        public string PRRefrenceNo { get; set; }
    }
}
