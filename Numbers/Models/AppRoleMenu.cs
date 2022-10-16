using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class AppRoleMenu
    {
        public int Id { get; set; }

        [Required]
        public virtual AppMenu Menu { get; set; }
        [Required]
        public virtual IdentityRole Role{ get; set; }
    }
}
