using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class AppMenu
    {


        public int Id { get; set; }
        [Required]
        [MaxLength(70)]
        public string Name { get; set; }
        [Required]
        [MaxLength(200)]
        public string Url { get; set; }
        [MaxLength(50)]
        [Display(Name = "Icon Class")]
        public string IconClass { get; set; }
        public int MenuLevel { get; set; }
        [MaxLength(10)]
        public string MenuType { get; set; }
        public int Sequence { get; set; }
        [Required]
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
        [Required]
        [Display(Name = "Has Sub Menus")]
        public bool HasSubMenus { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int ParentId { get; set; }

    }
}
