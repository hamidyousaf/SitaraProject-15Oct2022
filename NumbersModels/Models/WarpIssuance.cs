using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class WarpIssuance
    {
        [Key]
        public int Id { get; set; }
        public int YarnIssuanceId { get; set; }
        public int ItemId { get; set; }
        public int BrandId { get; set; }
        public int UOMId { get; set; }
        public decimal Quantity { get; set; }
        public InvItem Item { get; set; }
        public AppCompanyConfig Brand { get; set; }
        public AppCompanyConfig UOM { get; set; }
    }
}