using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class BomAccessoriesDetail
    {
        public int Id { get; set; }

        [ForeignKey("BomAccessories")]
        public int BOMId { get; set; }
        public int GeneralAccessoriesId { get; set; }
        public int ItemSpecifiedId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public int UOMId { get; set; }
        public int NatureId { get; set; }
        // navigational property
        public InvItem Item { get; set; }
        public AppCompanyConfig UOM { get; set; }
        public AppCompanyConfig Nature { get; set; }
    }
}
