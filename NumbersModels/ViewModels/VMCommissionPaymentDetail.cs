using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class VMCommissionPaymentDetail
    {
        public int Id { get; set; }
        public int CommissionPaymentId { get; set; }
        public int ReceiptId { get; set; }
        public DateTime ReceiptDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int Cityid { get; set; }
        public string CityName { get; set; }
        public Decimal ReceiptAmount { get; set; }
        public Decimal CommissionAmount { get; set; }
        public Decimal PaymentAmount { get; set; }
    }
}
