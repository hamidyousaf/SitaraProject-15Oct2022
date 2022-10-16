using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class ApplicationUser : IdentityUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int AppUserID { get; set; }
        [MaxLength(150)]
        public string FullName { get; set; }
        public bool IsMaster { get; set; }
        [MaxLength(500)]
        public string Photo { get; set; }
        public int CompanyId { get; set; }

        public bool AllDepartment { get; set; }

    }

    public class AspNetUserInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }

        public bool AllDepartment { get; set; }
    }

}
