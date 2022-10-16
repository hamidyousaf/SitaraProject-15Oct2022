using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class AppUserCompany
    {
        public int Id { get; set; }
        [Required]
        public AppCompany Company { get; set; }
        [Required]
        public ApplicationUser User { get; set; }
        [Required]
        public bool IsDefault { get; set; }
    }
}
