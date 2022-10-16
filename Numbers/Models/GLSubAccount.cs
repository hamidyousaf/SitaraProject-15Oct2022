using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class GLSubAccount
    {
        public int Id { get; set; }
        [MaxLength(10)]
        [Required(ErrorMessage = "Please enter Prefix")]
        public string Code { get; set; }
        [MaxLength(100)]
        [Required(ErrorMessage = "Please enter Description")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public AppCompany Company { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
