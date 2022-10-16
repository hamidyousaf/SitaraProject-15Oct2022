using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class BillOfMaterialItem
    {
        [Key]
        public int Id { get; set; }
        public int BillOfMaterialId { get; set; }
     
        public int ItemId { get; set; }
        public int UOMId { get; set; }
        public int AvailableStock { get; set; }
        public int RequiredQuantity { get; set; }
        public string Category { get; set; }

    }
}