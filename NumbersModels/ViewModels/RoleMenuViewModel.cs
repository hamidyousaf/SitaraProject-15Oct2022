using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.ViewModels
{
    public class RoleMenuViewModel
    {
        [Required]
        [Display(Name = "User Roles")]
        public string UserRoles { get; set; }


    }
}
