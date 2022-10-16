using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.ViewModels
{
   public class SubAccountVM
    {

        public GLSubAccount GLSubAccount { get; set; }

        public List<GLSubAccountDetail> GLSubAccountDetails { get; set; }
        public GLSubAccountDetail GLSubAccountDetail { get; set; }
        public string CreatedBy { get; set; }
        public string ShortDate { get; set; }
        public string Account { get; set; }
        public string Id { get; set; }
        public string Status { get; set; }
    }  
}
