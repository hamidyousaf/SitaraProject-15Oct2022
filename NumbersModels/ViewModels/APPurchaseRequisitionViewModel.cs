using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Numbers.Entity.Models;

namespace Numbers.Entity.ViewModels
{
    public class APPurchaseRequisitionViewModel
    {
        public APPurchaseRequisition APPurchaseRequisition { get; set; }
        public string DepartmentName { get; set; }
        public string UserName { get; set; }
        public string PrDate { get; set; }
        public string UOM { get; set; }
        public List<string> LastPOdate { get; set; }
        public List<decimal> Stock { get; set; }
        public List<string> UOmName { get; set; }
        public List<APPurchaseRequisitionDetails> APPurchaseRequisitionDetails { get; set; }

       


    }
}
