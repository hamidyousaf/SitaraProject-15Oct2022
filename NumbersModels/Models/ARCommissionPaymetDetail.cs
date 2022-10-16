using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
   public class ARCommissionPaymetDetail
    {
        public int Id { get; set; }
        [ForeignKey("CommissionPayment")]
        public int CommissionPaymentId { get; set; }
        public int ReceiptId { get; set; }
        public DateTime ReceiptDate { get; set; }
        public int CustomerId { get; set; }
        public int CategoryId { get; set; }
        public int Cityid { get; set; }
        public Decimal ReceiptAmount { get; set; }
        public Decimal CommissionAmount { get; set; }
        public Decimal PayAmount { get; set; }
        public bool IsDeleted { get; set; }

        // public AppCitiy City { get; set; }
        //public ARCustomer Customer { get; set; }
        //public InvItemCategories Category { get; set; }
      
       public ARCommissionPayment CommissionPayment { get; set; }
    }
}
