using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class AppUserCompany
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        public AppCompany Company { get; set; }
        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [Required]
        public bool IsDefault { get; set; }
    }
}
