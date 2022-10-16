using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Numbers.Entity.Models
{
    public class AppAuditTrial
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string UserFullName { get; set; }
        [MaxLength(450)]
        public string UserId { get; set; }
        public int CompanyId { get; set; }
        public AppCompany Company { get; set; }
        [MaxLength(30)]
        public string Module { get; set; }
        [MaxLength(30)]
        public string SourceTable { get; set; }
        public string SourceField { get; set; }
        [MaxLength(100)]
        public string TransactionId { get; set; }
        [MaxLength(100)]
        public string IP { get; set; }
        public string BeforeValue { get; set; }
        public string AfterValue { get; set; }
        public DateTime? CreatedDate { get; set; }
        [MaxLength(100)]
        public string Action { get; set; }


    }
}
