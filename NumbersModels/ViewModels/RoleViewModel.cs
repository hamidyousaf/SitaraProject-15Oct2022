using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.ViewModels
{
    public class RoleViewModel 
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public int MenuId { get; set; }
        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
        public string Menus { get; set; } //JSON data of JS Tree
        public int CompanyId { get; set; }

    }
}
