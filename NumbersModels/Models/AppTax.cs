using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class AppTax
    {
        public int Id { get; set; }
        [Display(Name = "Name")] [MaxLength(100)] public string Name { get; set; }
        [Display(Name = "Sales Tax.%")] public decimal SalesTaxPercentage { get; set; }
        [Display(Name = "Sales Tax Account")] public int SalesTaxAccountId { get; set; }
        [Display(Name = "Excise Tax.%")] public decimal ExciseTaxPercentage { get; set; }
        [Display(Name = "Excise Tax Account")] public int ExciseTaxAccountId { get; set; }
        [Display(Name = "Income Tax.%")] public decimal IncomeTaxPercentage { get; set; }
        [Display(Name = "Income Tax Account")] public int IncomeTaxAccountId { get; set; }
        [Display(Name = "Description")] [MaxLength(450)] public string Description { get; set; }
        [Display(Name = "Company Id")] public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Active")] public bool IsActive { get; set; }
        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Created Date")][DataType(DataType.Date)]public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }
        [Display(Name = "Updated Date")] [DataType(DataType.Date)] public DateTime UpdatedDate { get; set; }

        //navigation property for foriegn key
        public GLAccount SalesTaxAccount { get; set; }
        public GLAccount ExciseTaxAccount { get; set; }
        public GLAccount IncomeTaxAccount { get; set; }

    }
}
