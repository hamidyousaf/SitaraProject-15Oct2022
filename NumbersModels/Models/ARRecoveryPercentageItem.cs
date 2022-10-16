using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Numbers.Entity.Models
{
    public class ARRecoveryPercentageItem
    {
        public int Id { get; set; }
        [ForeignKey("ARRecoveryPercentage")]
        public int RecoveryPercentage_Id { get; set; }

        [Column(TypeName = "numeric(18,4)")]
        public decimal FromPerc{ get; set; }

        [Column(TypeName = "numeric(18,4)")]
        public decimal ToPerc { get; set; }
        public int CategoryType_Id { get; set; }
        [NotMapped]
        public string CategoryType { get; set; }

        public virtual ARRecoveryPercentage ARRecoveryPercentage { get; set; }
    }
}
