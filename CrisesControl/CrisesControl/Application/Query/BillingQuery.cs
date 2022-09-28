using AutoMapper;
using CrisesControl.Api.Application.Commands.Billing.GetAllInvoices;
using CrisesControl.Api.Application.Commands.Billing.GetBillingSummary;
using CrisesControl.Api.Application.Commands.Billing.GetInvoicesById;
using CrisesControl.Api.Application.Commands.Billing.GetInvSchedule;
using CrisesControl.Api.Application.Commands.Billing.GetOrders;
using CrisesControl.Api.Application.Commands.Billing.GetPaymentProfile;
using CrisesControl.Api.Application.Commands.Billing.GetTransactionDetails;
using CrisesControl.Api.Application.Commands.Billing.GetUnbilledSummary;
using CrisesControl.Api.Application.Commands.Billing.GetUsageGraph;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Billing.Repositories;

namespace CrisesControl.Api.Application.Query {
    public class BillingQuery : IBillingQuery  {
        private readonly IBillingRepository _billingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BillingQuery> _logger;

        public BillingQuery(IBillingRepository billingRepository, IMapper mapper,
            ILogger<BillingQuery> logger) {
            _logger = logger;
            _mapper = mapper;
            _billingRepository = billingRepository;
        }

        public async Task<GetPaymentProfileResponse> GetPaymentProfile(GetPaymentProfileRequest request) {
            var p_profile = await _billingRepository.GetPaymentProfile(request.CompanyId);
            GetPaymentProfileResponse result = _mapper.Map<BillingPaymentProfile, GetPaymentProfileResponse>(p_profile);
            return result;
        }

        public async Task<List<GetBillingSummaryResponse>> GetBillingSummary(GetBillingSummaryRequest request)
        {
            var billingSummary = await _billingRepository.GetBillingSummary(request.OutUserCompanyId,request.ChkUserId);
            List<GetBillingSummaryResponse> result = _mapper.Map<List<GetBillingSummaryResponse>>(billingSummary);
            return result;
        }

        public async Task<GetAllInvoicesResponse> GetAllInvoices(GetAllInvoicesRequest request)
        {
            var invoices = await _billingRepository.GetAllInvoices(request.CompanyId);
            GetAllInvoicesResponse result = _mapper.Map<GetAllInvoicesResponse>(invoices);
            return result;
        }

        public async Task<List<GetInvScheduleResponse>> GetInvSchedule(GetInvScheduleRequest request)
        {
            var invoiceItems = await _billingRepository.GetInvItems(request.OrderId, request.MonthVal, request.YearVal);
            List<GetInvScheduleResponse> result = _mapper.Map<List<GetInvScheduleResponse>>(invoiceItems);
            return result;
            
        }

        public async Task<List<GetOrdersResponse>> GetOrders(GetOrdersRequest request)
        {
            var orderList = await _billingRepository.GetOrder(request.OrderId, request.CompanyId,request.CustomerId, request.OriginalOrderId);
            List<GetOrdersResponse> result = _mapper.Map<List<GetOrdersResponse>>(orderList);
            return result;

        }

        public async Task<GetInvoicesByIdResponse> GetInvoicesById(GetInvoicesByIdRequest request)
        {
            var invoices = await _billingRepository.GetInvoicesById(request.CompanyId,request.TransactionHeaderId,request.ShowPayments);
            GetInvoicesByIdResponse result = _mapper.Map<GetInvoicesByIdResponse>(invoices);
            return result;
        }

        public async Task<List<GetTransactionDetailsResponse>> GetTransactionDetails(GetTransactionDetailsRequest request)
        {
            var transactions = await _billingRepository.GetTransactionItem(request.companyId, request.messageId, request.method, request.recordStart, request.recordLength, request.searchString, request.orderBy, request.orderDir, request.companyKey);
            List<GetTransactionDetailsResponse> result = _mapper.Map<List<GetTransactionDetailsResponse>>(request);
            return result;
        }

        public async Task<List<GetUsageGraphResponse>> GetUsageGraph(GetUsageGraphRequest request)
        {
            var usageGraph = await _billingRepository.GetUsageGraph(request.CompanyId, request.ReportType, request.LastMonth);
            List<GetUsageGraphResponse> result = _mapper.Map<List<GetUsageGraphResponse>>(usageGraph);
            return result;
        }

        public async Task<List<GetUnbilledSummaryResponse>> GetUnbilledSummary(GetUnbilledSummaryRequest request)
        {
            var unbilledSummary = new List<UnbilledSummary>();

            if (request.ReportType.ToUpper() == "SUMMARY")
            {
                unbilledSummary = await _billingRepository.GetUnbilledSummary(request.StartYear);
            }
            else if (request.ReportType.ToUpper() == "MONTH")
            {
                unbilledSummary = await _billingRepository.GetUnbilledSummaryByMonth(request.StartYear, request.StartMonth);
            }
            else if (request.ReportType.ToUpper() == "MESSAGE")
            {
                unbilledSummary = await _billingRepository.GetUnbilledSummaryByMessage(request.MessageId);
            }
            List<GetUnbilledSummaryResponse> result = _mapper.Map<List<GetUnbilledSummaryResponse>>(unbilledSummary);
            return result;
        }

    }
}
