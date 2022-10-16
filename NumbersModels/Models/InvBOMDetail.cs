using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
   public class InvBOMDetail
    {
        public int Id { get; set; }
        public int BOMId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public int UOMId { get; set; }
        public int NatureId { get; set; }
        // navigational property
        [ForeignKey("ItemId")]
        public InvItem Item { get; set; }
        [ForeignKey("UOMId")]
        public AppCompanyConfig UOM { get; set; }
        [ForeignKey("NatureId")]
        public AppCompanyConfig Nature { get; set; }

    }
}
