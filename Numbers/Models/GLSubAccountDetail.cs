using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class GLSubAccountDetail
    {
        public int Id { get; set; }
        public GLSubAccount SubAccountId { get; set; }
        [MaxLength(10)]
        [Required(ErrorMessage ="Please enter Prefix")]
        public string Code { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessage = "Please enter description")]
        public string Description { get; set; }
        public bool IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
