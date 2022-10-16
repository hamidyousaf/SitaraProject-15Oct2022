using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.ViewModels
{
    public class UserViewModel
    {
        [MaxLength(450)]
        public string Id { get; set; }
        //public int? AppUserID { get; set; }
        public string FullName { get; set; }
        public bool IsMaster { get; set; }
        public string Photo { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
        public int CompanyId { get; set; }

        
    }
   

    public class GroupedUserViewModel
    {
        public List<UserViewModel> Users { get; set; }
        //public List<UserViewModel> Admins { get; set; }
    }
     


}
