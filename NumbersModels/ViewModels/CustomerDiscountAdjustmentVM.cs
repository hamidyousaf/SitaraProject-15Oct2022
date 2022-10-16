using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.ViewModels
{
   public class CustomerDiscountAdjustmentVM
    {
        public ARCustomerDiscountAdjustment CustomerDiscountAdjustment { get; set; }

        public List<ARCustomerAdjustmentItem> CustomerAdjustmentItem { get; set; }
        public int ARDiscountId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryDesc { get; set; }
        public int Customer_Id { get; set; }
        public string CustomerDesc { get; set; }
        public int SalesPersonId { get; set; }
        public string SalesPersonDesc { get; set; }
        public int CityId { get; set; }
        public string CityDesc { get; set; }
        [Column(TypeName =("numeric(18,4)"))]
        public decimal GrandTotal { get; set; }
        public decimal DiscountUtilize { get; set; }
        [Column(TypeName = ("numeric(18,4)"))]
        public decimal DiscountBalance { get; set; }
        public string CustomerName { get; set; }
        public string TransactionType { get; set; }
        public int TransactionNo { get; set; }
        [Column(TypeName = ("numeric(18,4)"))]
        public decimal DiscountAmount { get; set; }
        [Column(TypeName = ("numeric(18,4)"))]
        public decimal Utilized { get; set; }
        public int ARCustomerAdjustmentItemId { get; set; }
        public string Date { get; set; }
        public string TransType { get; set; }
        public bool? IsClosed { get; set; }
    }
}

