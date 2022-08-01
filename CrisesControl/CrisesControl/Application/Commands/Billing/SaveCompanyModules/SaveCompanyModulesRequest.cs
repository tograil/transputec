using CrisesControl.Api.Application.Commands.Billing.CreateOrder;
using CrisesControl.Core.Billing;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.SaveCompanyModules
{
    public class SaveCompanyModulesRequest : IRequest<SaveCompanyModulesResponse>
    {
        public int OrderId { get; set; }
        public int ContractDuration { get; set; }
        public decimal ContractValue { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public int KeyholderCount { get; set; }
        public int StaffCount { get; set; }
        public string OrderStatus { get; set; }
        public string TigerOrderNo { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal NetTotal { get; set; }
        public decimal VatTotal { get; set; }
        public DateTime ContractStartDate { get; set; }
        public List<Product> Products { get; set; }
        public string ContractType { get; set; }
        public int Activated { get; set; }
        public decimal Discount { get; set; }
    }
}
