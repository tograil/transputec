using CrisesControl.Api.Application.Commands.Shared;

namespace CrisesControl.Api.Application.Commands.Billing.GetBillingSummary
{
    public class GetBillingSummaryResponse: CommonModel
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
    }
}
