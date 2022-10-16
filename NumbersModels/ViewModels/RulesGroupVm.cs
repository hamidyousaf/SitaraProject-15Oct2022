using System;
using System.Collections.Generic;
using System.Text;
using Numbers.Entity.Models;


namespace Numbers.Entity.ViewModels
{
    public class RulesGroupVm
    {
        public Sys_Rules_Approval Sys_Rules_Approval { get; set; }
        public int Group_Id { get; set; }
        public int Type_Id { get; set; }
        //public Sys_Rules Sys_Rules { get; set; }

        //public  List<Sys_RulesDetails> Sys_RulesDetails { get; set; }
        public List<Sys_Rules_Approval_Details> Sys_Rules_Approval_Details { get; set; }
    }
}
