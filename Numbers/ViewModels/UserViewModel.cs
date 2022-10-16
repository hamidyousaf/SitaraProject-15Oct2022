using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public int AppUserID { get; set; }
        public string FullName { get; set; }
        public bool IsMaster { get; set; }
        public string Photo { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
    }

    public class GroupedUserViewModel
    {
        public List<UserViewModel> Users { get; set; }
        //public List<UserViewModel> Admins { get; set; }
    }


}
