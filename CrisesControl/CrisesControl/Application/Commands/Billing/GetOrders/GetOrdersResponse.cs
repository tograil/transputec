using CrisesControl.Core.Billing;

namespace CrisesControl.Api.Application.Commands.Billing.GetOrders
{
    public class GetOrdersResponse
    {
        public int OrderID { get; set; }
        public string CustomerId { get; set; }
        public int CompanyId { get; set; }
        public int ContractDuration { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public double ContractValue { get; set; }
        public string PaymentMethod { get; set; }
        public string TigerOrderNo { get; set; }
        public string InvoiceNumber { get; set; }
        public string Status { get; set; }
        public int KeyholderCount { get; set; }
        public int StaffCount { get; set; }
        public double NetTotal { get; set; }
        public double VatTotal { get; set; }
        public DateTime ContractStartDate { get; set; }
        public List<CompanyPackageFeatures> Modules { get; set; }
        public List<InvoiceSchReturn> InvItems { get; set; }
        public string ContractType { get; set; }
    }
}
