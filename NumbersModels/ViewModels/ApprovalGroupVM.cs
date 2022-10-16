using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class ApprovalGroupVM
    {
        public virtual SysApprovalGroup SysApprovalGroup { get; set; }
        //public SysApprovalGroupDetails SysApprovalGroupDetails { get; set; }

        //public List<SysApprovalGroup> SysApprovalGroupList { get; set; }
        public List<SysApprovalGroupDetails> SysApprovalGroupDetails { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}
