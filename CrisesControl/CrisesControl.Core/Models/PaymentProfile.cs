using System;

namespace CrisesControl.Core.Models
{
    public partial class PaymentProfile
    {
        public int PaymentProfileId { get; set; }
        public decimal MinimumBalance { get; set; }
        public int StorageLimit { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal TextUplift { get; set; }
        public decimal PhoneUplift { get; set; }
        public decimal EmailUplift { get; set; }
        public decimal PushUplift { get; set; }
        public decimal ConfUplift { get; set; }
        public decimal MinimumTextRate { get; set; }
        public decimal MinimumPhoneRate { get; set; }
        public decimal MinimumEmailRate { get; set; }
        public decimal MinimumPushRate { get; set; }
        public decimal MinimumConfRate { get; set; }
        public DateTimeOffset StatementRunDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public decimal SoptokenValue { get; set; }
        public decimal CreditBalance { get; set; }
    }
}
