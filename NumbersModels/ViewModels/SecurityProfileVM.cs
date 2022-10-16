using Microsoft.AspNetCore.Mvc.Rendering;
using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
    public class SecurityProfileVM
    {
        public string Classification { get; set; }
        public SelectList ClassificationList { get; set; }
        public virtual Sys_ORG_Profile sys_ORG_Profile { get; set; }
        public virtual List<Sys_ORG_Profile_Details> Sys_ORG_Profile_Details { get; set; }
        public virtual AppCompanyConfig appCompanyConfig { get; set; }
        
    }
}
