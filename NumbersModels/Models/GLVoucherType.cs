using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class GLVoucherType
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(5)]
        public string VoucherType { get; set; }
        [Required]
        [MaxLength(50)]
        public string Description { get; set; }
        [MaxLength(20)]
        public string ReferenceNarration { get; set; }
        [Required]
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
