using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
   public class AppCompanyConfigBase
    {
        public int Id { get; set; }
        [Display(Name = "Module")] [MaxLength(50)] public string Module { get; set; }
        [Display(Name = "Module")]public int ModuleId { get; set; }
        [Display(Name = "Module")] public string Name { get; set; }
        [Display(Name = "Description")] [MaxLength(200)] public string Description { get; set; }
        public string MaxSize { get; set; }
        public int CompanyId { get; set; }
        [MaxLength(40)] public string Type { get; set; }

        public bool IsChild { get; set; }
        public bool IsSystem { get; set; }
        [Display(Name = "Active")] public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
