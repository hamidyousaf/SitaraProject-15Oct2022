using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class AppReportQueueParameters
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string TransactionType { get; set; }
        public string QueueId { get; set; }
    }
}
