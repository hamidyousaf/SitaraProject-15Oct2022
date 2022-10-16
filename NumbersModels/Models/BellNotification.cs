using System;
using System.Collections.Generic;
using System.Text;

namespace Numbers.Entity.Models
{
    public class BellNotification
    {
        public string TotalFollowUP { set; get; }
        public string TotlaCounselor { set; get; }
        public string TotalApplication { set; get; }
        public string TotalUnApproveApplication { set; get; }
        public string TotalAcceptance { set; get; }
        public string TotalVisa { set; get; }
        public string TotalNotification { set; get; }

        // Visa Manger
        public string TotalPendingAcceptance { set; get; }
        public string TotlaAcceptanceApplication { set; get; }
        public string TotalVisaApplication { set; get; }
        public string TotalGrant { set; get; }
        public string TotalVisaRefused { set; get; }
        public string TotalPendingVisa { set; get; }

        public string TotalUnApproveReceipt { set; get; }
        public string TotalUnApprovePayment { set; get; }

    }
}
