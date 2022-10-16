using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class AppCompanyConfig
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        [MaxLength(50)]
        public string Module { get; set; }
        [MaxLength(50)]
        public string ConfigName { get; set; }
        [MaxLength(200)]
        public string ConfigValue { get; set; }
    }
}
