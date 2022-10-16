using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class AppCurrency
    {
        [MaxLength(3)]
        public string Id { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Currency name is required")]
        public string Name { get; set; }
        [MaxLength(5)]
        public string Symbol { get; set; }
        public Int16 Sequence { get; set; }
        public bool IsActive { get; set; }
    }
}
