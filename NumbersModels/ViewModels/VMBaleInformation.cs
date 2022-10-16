using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.ViewModels
{
   public class VMBaleInformation
    {
        public int Id { get; set; }
        public int TransactionNo { get; set; }
        public string TransactionDate { get; set; }
        public string Level2 { get; set; }
        public string level3 { get; set; }
        public string level4 { get; set; }
        public string ItemId { get; set; }
        public string BaleType { get; set; }
        public string BaleMeter { get; set; }
        public string BaleNo { get; set; }
        public string ProductionOrderNo { get; set; }
        
    }

}
