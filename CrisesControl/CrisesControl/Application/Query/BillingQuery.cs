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
using CrisesControl.Api.Application.Helpers;

namespace CrisesControl.Api.Application.Query {
    public class BillingQuery : IBillingQuery  {
        private readonly IBillingRepository _billingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BillingQuery> _logger;
        private readonly ICurrentUser _currentUser;

        public BillingQuery(IBillingRepository billingRepository, IMapper mapper,
            ILogger<BillingQuery> logger, ICurrentUser currentUser) {
            _mapper = mapper;
            _billingRepository = billingRepository;
            _logger = logger;
            _currentUser = currentUser;

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
            var invoices = await _billingRepository.GetAllInvoices(_currentUser.CompanyId);
            GetAllInvoicesResponse result = _mapper.Map<GetAllInvoicesResponse>(invoices);
            return result;
        }

        public async Task<GetInvScheduleResponse> GetInvSchedule(GetInvScheduleRequest request)
        {
            var invoiceItems = await _billingRepository.GetInvItems(request.OrderId, request.MonthVal, request.YearVal);
            var result = _mapper.Map<List<InvoiceSchReturn>>(invoiceItems);
            var response = new GetInvScheduleResponse();
            if (result!=null)
            {
                response.Data = result;
            }
            else
            {
                response.Data = new List<InvoiceSchReturn>();
            }
            return response;
            
        }

        public async Task<GetOrdersResponse> GetOrders(GetOrdersRequest request)
        {
            var orderList = await _billingRepository.GetOrders(request.OrderId, _currentUser.CompanyId);
            var result = _mapper.Map<List<OrderListReturn>>(orderList);
            var response = new GetOrdersResponse();
            response.Data = result;
            return response;

        }

        public async Task<GetInvoicesByIdResponse> GetInvoicesById(GetInvoicesByIdRequest request)
        {
            var invoices = await _billingRepository.GetInvoicesById(_currentUser.CompanyId,request.TransactionHeaderId,request.ShowPayments);
            GetInvoicesByIdResponse result = _mapper.Map<GetInvoicesByIdResponse>(invoices);
            return result;
        }

        public async Task<GetTransactionDetailsResponse> GetTransactionDetails(GetTransactionDetailsRequest request)
        {
            var transactions = await _billingRepository.GetTransactionItem(_currentUser.CompanyId, request.messageId, request.method, request.recordStart, request.recordLength, request.searchString, request.orderBy, request.orderDir, request.companyKey);
            var result = _mapper.Map<List<TransactionItemDetails>>(transactions);
            var response = new GetTransactionDetailsResponse();
            response.Details = result;
            return response;
        }

        public async Task<GetUsageGraphResponse> GetUsageGraph(GetUsageGraphRequest request)
        {
            var usageGraph = await _billingRepository.GetUsageGraph(_currentUser.CompanyId, request.ReportType, request.LastMonth);
            var  result = _mapper.Map<List<TransactionItemDetails>>(usageGraph);
            var response = new GetUsageGraphResponse();
            response.Details = result;
            return response;
        }

        public async Task<GetUnbilledSummaryResponse> GetUnbilledSummary(GetUnbilledSummaryRequest request)
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
            var result = _mapper.Map<List<UnbilledSummary>>(unbilledSummary);           
            var response = new GetUnbilledSummaryResponse();
            response.unbilledSummaries = result;
            return response;
        }

    }
}
