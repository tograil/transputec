using System;
using System.Collections.Generic;

namespace CC.Authority.Implementation.Models
{
    public partial class CompanyPaymentProfile
    {
        public int CompanyPaymentProfileId { get; set; }
        public DateTimeOffset ContractAnniversary { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string? PaymentPeriod { get; set; }
        public DateTimeOffset LastCreditDate { get; set; }
        public decimal CreditBalance { get; set; }
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
        public DateTimeOffset ContractStartDate { get; set; }
        public int CompanyId { get; set; }
        public DateTimeOffset LastStatementEndDate { get; set; }
        public DateTimeOffset CurrentStatementEndDate { get; set; }
        public string? AgreementNo { get; set; }
        public decimal SoptokenValue { get; set; }
        public decimal MaxTransactionLimit { get; set; }
        public DateTimeOffset CardExpiryDate { get; set; }
        public bool CardFailed { get; set; }
        public string? CardType { get; set; }
        public string? BillingAddress1 { get; set; }
        public string? BillingAddress2 { get; set; }
        public string? City { get; set; }
        public string? Town { get; set; }
        public string? Postcode { get; set; }
        public string? Ipaddress { get; set; }
        public string? Country { get; set; }
        public string? BillingEmail { get; set; }
        public string? CardHolderName { get; set; }
        public decimal? Vatrate { get; set; }
        public bool? OrderCustomer { get; set; }
    }
}
