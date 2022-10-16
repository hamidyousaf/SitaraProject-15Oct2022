using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class InvItemAccount
    {

        public int Id { get; set; }
        [Display(Name = "Asset Account")]
        public int GLAssetAccountId { get; set; }
        [Display(Name = "Sale Account")]
        public int GLSaleAccountId { get; set; }
        [Display(Name = "Cost of Sale Account")]
        public int GLCostofSaleAccountId { get; set; }
        [Display(Name = "Work In Progress Account")]
        public int GLWIPAccountId { get; set; }
        [Required]
        [Display(Name = "Company Id")] public int CompanyId { get; set; }

        [MaxLength(450)]
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Account Name")]
        public string Name { get; set; }
        [Display(Name = "Updated By")]
        [Column(TypeName = "nvarchar(450)")]
        public string UpdatedBy { get; set; }

        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Updated Date")]
        [DataType(DataType.Date)]
        public DateTime UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        //navigation property
        public GLAccount GLAssetAccount { get; set; }
        public GLAccount GLSaleAccount { get; set; }
        public GLAccount GLCostofSaleAccount{ get; set; }
        public GLAccount GLWIPAccount { get; set; }
}
}
