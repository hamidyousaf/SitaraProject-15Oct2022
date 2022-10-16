using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GLDebitCreditMemoDetail
    {
        public int Id { get; set; }
        public int GLDebitCreditMemoId { get; set; }
        [Display(Name = "Account Name")]
        public int GLAccountId { get; set; }
        [Display(Name = "Sub Account")]
        public int SubAccountId { get; set; }
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        [Display(Name = "Sub Department")]
        public int SubDepartmentId { get; set; }
        [Display(Name = "Cost Center")]
        public int CostCenterId { get; set; }
        public string Remarks { get; set; }
        public decimal Total { get; set; }
        [Display(Name = "Discount %")]
        public decimal DiscountPercentage { get; set; }
        [Display(Name = "Discount Amount")]
        public decimal DiscountAmount { get; set; }
        [Display(Name ="Tax")]
        public int TaxId { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Line Total")]
        public decimal LineTotal { get; set; }
        // navigational property
        public GLDebitCreditMemo GLDebitCreditMemo { get; set; }
    }
}