using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Numbers.Models
{
    public class AppPackage
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public int Name { get; set; }
        [MaxLength(100)]
        public string Module { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
