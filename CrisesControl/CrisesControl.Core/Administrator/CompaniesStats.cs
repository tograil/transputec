using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class CompaniesStats
    {

        public int ActiveCompanies { get; set; }
        public int DeletedCompanies { get; set; }
        public int InactiveCompanies { get; set; }
        public int AwaitingPayment { get; set; }
        public int AwaitingVerification { get; set; }
        public int LowBalance { get; set; }
        public int OnCredit { get; set; }
        public int OnTrial { get; set; }
        public int StoppedMessaging { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int DeletedUsers { get; set; }
        public int PendingVerification { get; set; }
        public int TotalAdmin { get; set; }
        public int TotalKeyholders { get; set; }
        public int TotalCalls { get; set; }
        public int TotalText { get; set; }
        public int TotalPush { get; set; }
        public int TotalEmail { get; set; }
        public decimal TotalCostIncurred { get; set; }
        public decimal TotalCostPhone { get; set; }
        public decimal TotalCostText { get; set; }
    }
}
