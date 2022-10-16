using Numbers.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.ViewModels
{
  public class AppMenuViewModel
    {
        //Master
        public int MENU_ID { get; set; }
        [Display(Name = "MENU NAME")]
        public string MENU_NAME { get; set; }
        [Display(Name = "USER MENU NAME")]
        public string USER_MENU_NAME { get; set; }
        [Display(Name = "DESCRIPTION")]
        public string DESCRIPTION { get; set; }
        [Display(Name = "MENU TYPE")]
        public int MENU_TYPE { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string LAST_UPDATED_BY { get; set; }
        public DateTime LAST_UPDATED_DATE { get; set; }

        //Detail
        public int MENU_M_ID { get; set; }
        public int MENU_D_ID { get; set; }
        public string PROMPTS { get; set; }
        public int SUBMENU_ID { get; set; }
        public int FUNCTION_ID { get; set; }
        public int SEQUENCE_ID { get; set; }

        //navigation Property
        public List<SYS_FORMS> Forms { get; set; }
        public List<SYS_MENU_D> InvStoreIssueItems { get; set; }


    }
}
