using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class APInsuranceInfo
    {
        [Key]
        public int Id { get; set; }
        public int LCId { get; set; }
        public decimal Charges { get; set; }
        public string CoverNoteNo{get;set;}
        public int InsuranceCompanyId { get; set; }
        public DateTime CoverNoteDate { get; set; }


    }
}
