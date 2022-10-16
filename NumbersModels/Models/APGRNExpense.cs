using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APGRNExpense
    {
        [Key]
        public int Id { get; set; }
        public int GRNId { get; set; }
        [Display(Name = "GL Code")] public string GLCode { get; set; }
        [Display(Name = "Account Name")] public string AccountName { get; set; }
        [Display(Name = "Expense Amount")] public decimal ExpenseAmount { get; set; }
        [Display(Name = "Total Amount")] public decimal TotalAmount { get; set; }

        [MaxLength(450)] public string CreatedBy { get; set; }
        [MaxLength(450)] public string UpdatedBy { get; set; }

         public DateTime UpdatedDate { get; set; }

         public DateTime CreatedDate { get; set; }

         public DateTime ExpiryDate { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int LCId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Remarks { get; set; }
        public string ExpenseFavour { get; set; }


    }
}
