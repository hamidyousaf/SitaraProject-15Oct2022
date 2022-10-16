using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APCSRequestDetail
    {
        public int ID { get; set; }
        public int APCSRequest_ID { get; set; }
        public int APComparativeStatement_ID { get; set; }
        public int Vendor_ID { get; set; }
        public int PaymentTerm { get; set; }
        public int DeliveryTerm { get; set; }
        public int SaleTax { get; set; }
        public int BrandId { get; set; }
        public decimal Qunatity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal Freight { get; set; }
        public decimal GrandTotal { get; set; }
        public bool IsEnable { get; set; }
        public int POType_ID { get; set; }
        public int Sequence { get; set; }
    }
}
