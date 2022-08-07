using CrisesControl.Core.Billing;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.CreateInvoiceSchedule
{
    public class CreateInvoiceScheduleRequest : IRequest<CreateInvoiceScheduleResponse>
    {
        public int CompanyId { get; set; }
        public string CustomerId { get; set; }
        public int OrderId { get; set; }
        public int ContractDuration { get; set; }
        public double ContractValue { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public int KeyholderCount { get; set; }
        public int StaffCount { get; set; }
        public string OrderStatus { get; set; }
        public string TigerOrderNo { get; set; }
        public string InvoiceNumber { get; set; }
        public double NetTotal { get; set; }
        public double VatTotal { get; set; }
        public DateTime ContractStartDate { get; set; }
        public List<Product> Products { get; set; }
    }
}
