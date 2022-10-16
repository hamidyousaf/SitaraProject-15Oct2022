using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class BkgItem
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Item Code is required")]
        public string ItemCode { get; set; }

        [Required]
        [MaxLength(150, ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [MaxLength(15)]
        public string UOM { get; set; }

        public IList<BkgVehicle> BkgVehicles { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
