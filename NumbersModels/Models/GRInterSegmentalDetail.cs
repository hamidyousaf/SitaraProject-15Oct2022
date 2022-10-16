using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class GRInterSegmentalDetail
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int UOMId { get; set; }
        public int BrandId { get; set; }
        public int AvlblQty { get; set; }
        public int SaleTrnsfrQty { get; set; }
        public int Rate { get; set; }
        public int Amount { get; set; }
        public string LotNo { get; set; }
        public int WeaverQty { get; set; }
        public int MendingQty { get; set; }
        public int FoldingQty { get; set; }
        public int GRInterSegmentalId { get; set; }
     
    }
}
