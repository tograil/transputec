using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Billing
{
    public class BillingSummaryModel
    {
        public double StorageUsed { get; set; }
        public double StorageLimit { get; set; }
        public int DocsCount { get; set; }
        public int AudioCount { get; set; }
        public int VideoCount { get; set; }
        public int UserLimit { get; set; }
        public int TotalPushMessage { get; set; }
        public int TotalEmailMessage { get; set; }
        public int TotalTextMessage { get; set; }
        public int TotalPhoneMessage { get; set; }
        public int MonthPushMessage { get; set; }
        public int MonthEmailMessage { get; set; }
        public int MonthTextMessage { get; set; }
        public int MonthPhoneMessage { get; set; }
        public int AdminCount { get; set; }
        public int KeyHolderCount { get; set; }
        public int StaffCount { get; set; }
        public int PendingUserCount { get; set; }
        public int ActiveUserCount { get; set; }
        public int StorageinGB { get; set; }
        public double AssetSize { get; set; }
        public DateTimeOffset Anniversary { get; set; }
        public DateTimeOffset ContractStartDate { get; set; }
        public decimal CreditBalance { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal MinimumBalance { get; set; }
        public string PaidServices { get; set; }

        public int ErrorId { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
