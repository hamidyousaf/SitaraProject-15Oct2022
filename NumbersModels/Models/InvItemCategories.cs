using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
   public class InvItemCategories
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [Display(Name ="Parent")]
        public int? ParentId { get; set; }
        public int CategoryLevel { get; set; }

        [Display(Name="Category")]
        [MaxLength(200)]
        public string Name { get; set; }
        [Display(Name = "Created By")]
        [MaxLength(450)]
        public string CreatedBy { get; set; }
        [Display(Name = "Updated By")]
        [MaxLength(450)]
        public string UpdatedBy { get; set; }

        [Display(Name="Active")]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime UpdatedDate { get; set; }

        public InvItemCategories Parent { get; set; }
        public string Code { get; set; }
         [NotMapped]
        public short AccountLevel { get; set; }
        [NotMapped]
        public string ParentCode { get; set; }
        [NotMapped]
        public int NewCodeLength { get; set; }
        public string Status { get; set; }
    }
}
