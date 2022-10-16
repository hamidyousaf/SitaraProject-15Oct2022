using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Numbers.Entity.Models
{
    public class AppReportQueue
    {
        [MaxLength(450)]
        public string Id { get; set; }
        public int ReportId { get; set; }
        public AppReport Report { get; set; }
        public string Parameters { get; set; }
        [MaxLength(450)]
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime PreviewDate { get; set; }
    }
}
