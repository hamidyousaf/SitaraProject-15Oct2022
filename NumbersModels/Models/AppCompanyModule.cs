using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class AppCompanyModule
    {
        [Key]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [MaxLength(1000)] [Display(Name = "Module Name")] public string Module_Name { get; set; }
        [MaxLength(500)] [Display(Name = "Module Description")] public string Module_Description { get; set; }
        [MaxLength(1000)] [Display(Name = "Short Name")] public string Short_Name { get; set; }

        [Display(Name = "Active ")] public bool Is_Active { get; set; }

        [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
