﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class HRFinalSettlementIncentive
    {
        public int Id { get; set; }
        //public int FinalSettlementId { get; set; }
        //public int IncentiveId { get; set; }

        [Display(Name = "Created By")] [MaxLength(450)] public string CreatedBy { get; set; }
        [Display(Name = "Updated By")] [MaxLength(450)] public string UpdatedBy { get; set; }

        [Column(TypeName = "numeric(18,2)")] public decimal IncentiveAmount { get; set; }

        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        //navigation property
        public HRFinalSettlement FinalSettlement { get; set; }
        public HRIncentive Incentive { get; set; }
    }
}
