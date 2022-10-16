using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
   public class SYS_MENU_M
    {
        [Key]
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
    }
}
