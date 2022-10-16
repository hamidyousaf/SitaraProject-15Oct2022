using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class AppCompanySetup
    {
        public int Id { get; set; }
        [Display(Name = "Name")] public string Name { get; set; }
        [Display(Name = "Value")] [MaxLength(450)] public string Value { get; set; }
        [Display(Name = "Data Type")] [MaxLength(50)] public string DataType { get; set; }
        [Display(Name = "Max Length")] public int MaxLength { get; set; }
        [Display(Name = "Validation Rule")] public string ValidationRule { get; set; }
        [Display(Name = "Required")] public bool IsRequired { get; set; }
        [Display(Name = "Module")] [MaxLength(20)] public string ModuleId { get; set; }
        public int CompanyId { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
