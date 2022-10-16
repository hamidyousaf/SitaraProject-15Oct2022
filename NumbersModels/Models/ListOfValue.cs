using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ListOfValue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int IRNNo { get; set; }
        public int TransactionNo { get; set; }
        public decimal Quantity { get; set; }
        public InvItem InvItem { get; set; }
        public string GreigeId { get; set; }
        public int SalesTaxAmount { get; set; }
        public int ExciseTaxAmount { get; set; }
        public int IncomeTaxAmount { get; set; }
        public int AmountWithTax { get; set; }
        public int AmountWithoutTax { get; set; }
        public decimal dSalesTaxAmount { get; set; }
        public decimal dExciseTaxAmount { get; set; }
        public decimal dIncomeTaxAmount { get; set; }
        public decimal dAmountWithTax { get; set; }
        public string Code { get; set; }
        public string Barcode { get; set; }

    }
}
