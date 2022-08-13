using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Payments
{
    public class UpdateCompanyPaymentProfileModel
    {
        public decimal ContractValue { get; set; }
        public DateTimeOffset ContractAnniversary { get; set; }
        public string PaymentPeriod { get; set; }
        public DateTimeOffset ContractStartDate { get; set; }
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
        public DateTimeOffset LastInvoicedate { get; set; }
        public double LastInvoiceValue { get; set; }
        public DateTimeOffset LastPaymentDate { get; set; }
        public string AgreementNo { get; set; }
        public decimal MaxTransactionLimit { get; set; }
        public string CardType { get; set; }
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string IPAddress { get; set; }
        public string Country { get; set; }
        public string BillingEmail { get; set; }
        public string CardHolderName { get; set; }
        public bool AgreementRegistered { get; set; }
        public int ContractTransactionId { get; set; }
        public string PaymentMethod { get; set; }
        public string CompanyProfile { get; set; }
        public string TransactionCode { get; set; }
        public string WTransactionID { get; set; }
        [NotMapped]
        public int[] Modules { get; set; }
        public bool OnTrial { get; set; }
    }
}
