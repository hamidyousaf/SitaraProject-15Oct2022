using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class GLSubAccountDetail
    {
        public int Id { get; set; }
        public GLSubAccount SubAccountId { get; set; }
        [MaxLength(10)]
        [Required(ErrorMessage ="Please enter Prefix")]
        public string Code { get; set; }
        [MaxLength(1000)]
        [Required(ErrorMessage = "Please enter description")]
        public string Description { get; set; }
        public bool IsDelete { get; set; }
        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [ForeignKey("SubAccountId")]
        public int GLSubAccountId { get; set; }
        public int LCID { get; set; }
        [NotMapped]
        public string LCNo { get; set; }
        public bool IsActive { get; set; }
    }
}
