using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Entity.Models
{
    public class AppCurrencyExchangeRate
    {
        public int Id { get; set; }
        public AppCurrency Currency { get; set; }
        public AppCompany Company { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ExchangeRate { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CraetedDate { get; set; }
        [MaxLength(450)]
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
