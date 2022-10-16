using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class AppRoleMenu
    {
        public int Id { get; set; }
       
        [Required]
        public virtual AppMenu Menu { get; set; }
        public int MenuId { get; set; }
        [Required]
        public string RoleId { get; set; }
        public virtual IdentityRole Role { get; set; }

        //public bool isRead { get; set; }
        //public bool isAdd { get; set; }
        //public bool FullControl { get; set; }

    }
}
