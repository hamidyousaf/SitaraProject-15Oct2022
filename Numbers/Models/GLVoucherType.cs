using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class GLVoucherType
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(4)]
        public int VoucherType { get; set; }
        [Required]
        [MaxLength(50)]
        public int Description { get; set; }
        [MaxLength(20)]
        public int ReferenceNarration { get; set; }
        [Required]
        public int CompanyId { get; set; }
        public int IsActive { get; set; }
        public int IsSystem { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
