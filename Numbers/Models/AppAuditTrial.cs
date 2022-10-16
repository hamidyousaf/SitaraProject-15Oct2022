using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class AppAuditTrial
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string UserFullName { get; set; }
        public int UserId { get; set; }
        public AppCompany Company { get; set; }
        [MaxLength(30)]
        public string Module { get; set; }
        [MaxLength(30)]
        public string TransactionType { get; set; }
        public int TransactionId { get; set; }
        [MaxLength(100)]
        public string SourceField { get; set; }
        public string BeforeValue { get; set; }
        public string AfterValue { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
