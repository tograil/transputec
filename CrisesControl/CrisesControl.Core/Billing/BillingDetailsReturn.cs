using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Billing
{
    public class BillingDetailsReturn
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }
        public string UserName { get; set; }
        public string SwitchPhone { get; set; }
        public string PrimaryEmailId { get; set; }
        public int InvoiceNo { get; set; }
        public DateTimeOffset StatementDate { get; set; }
        public decimal InvoiceTotal { get; set; }
        public string InvoiceStatus { get; set; }
        public decimal SubTotal { get; set; }
        public decimal VatRate { get; set; }
        public decimal VATValue { get; set; }
        public decimal CreditBalance { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal MinimumBalance { get; set; }
        public string TopUp { get; set; }
        public int AdminLimit { get; set; }
        public int AdminUsers { get; set; }
        public int StaffLimit { get; set; }
        public int StaffUsers { get; set; }
        public int StorageLimit { get; set; }
        public double StorageSize { get; set; }
        public DateTimeOffset BillingStartDate { get; set; }
        public DateTimeOffset BillingEndDate { get; set; }
        public List<BillingDetailsInfo> BillingDetailsInfoList { get; set; }
    }

    public class BillingDetailsInfo
    {
        public int TransactionDetailsId { get; set; }
        public decimal TransactionRate { get; set; }
        public string TransactionDescription { get; set; }
        public string TransactionCode { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public int MessageId { get; set; }
        public string MessageType { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public decimal Vat { get; set; }
        public string DRCR { get; set; }
        public string TransactionRef { get; set; }
    }
}
