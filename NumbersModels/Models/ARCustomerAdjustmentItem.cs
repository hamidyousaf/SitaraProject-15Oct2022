using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARCustomerAdjustmentItem
    {
        public int Id { get; set; }
        public int DiscountAdjustment_Id { get; set; }

        [Column(TypeName = "numeric(18,4)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "numeric(18,4)")]
        public decimal PaidAmount { get; set; }

        [Column(TypeName = "numeric(18,4)")]
        public decimal TransferAmount { get; set; }

        [Column(TypeName = "numeric(18,4)")]
        public int TransferToCustomer { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        public decimal DiscountBalance { get; set; }
        public int Customer_Id  { get; set; }
        public int ARDiscount_Id { get; set; }
        [Column(TypeName ="numeric(18,4)")]
        public decimal Utilized { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? UnApprovedBy { get; set; }
        public DateTime? UnApprovedDate { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDeleted { get; set; }
        [NotMapped]
        public string CustomerName { get; set; }
        public bool IsClosed { get; set; }
        public int VoucherId { get; set; }
        public ARCustomer Customer_ { get; set; }
      
    }
}
