using Castle.Components.DictionaryAdapter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace Numbers.Entity.Models
{
     
    public class SYS_FORMS
    {
        [Key]
        public int FORMID { get; set; }
        [Display(Name = "FORM MANUAL TITLE")]
        public string FORM_MANUAL_TITLE { get; set; }
        [Display(Name = "FORM FMX NAME")]
        public string FORM_FMX_NAME { get; set; }
        [Display(Name = "SHORT CODE")]
        public string SHORT_CODE { get; set; }
        [Display(Name = "DESCRIPTION")]
        public string FORM_DESCRIPTION { get; set; }
        [Display(Name = "MODULE")]
        public int MODULE_ID { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }
}
