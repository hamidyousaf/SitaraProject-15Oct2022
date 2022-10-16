using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
   public class SYS_MENU_D
    {
        [Key]
        public int MENU_D_ID { get; set; }
        public int MENU_M_ID { get; set; }
        public string PROMPTS { get; set; }
        public int SUBMENU_ID { get; set; }
        public int FUNCTION_ID { get; set; }
        public string DESCRIPTION { get; set; }
        public int SEQUENCE_ID { get; set; }

        //Navigation Property
        public AppMenu SUBMENU_ { get; set; }
    }
}
