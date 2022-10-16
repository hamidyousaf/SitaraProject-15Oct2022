using System;
using System.Collections.Generic;
using System.Text;
using Numbers.Entity.Models;
namespace Numbers.Entity.ViewModels
{
    public class ARDiscountViewModel
    {
        public ARDiscount ARDiscount { get; set; }
        public List<ARDiscountItem> ARDiscountItem { get; set; }
        public string EndDate { get; set; }
        public string StartDate { get; set; }
        public string CategoryName { get;set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Trans { get; set; }
        public string Status { get; set; }
        public List<string> Category { get; set; }
    }
}