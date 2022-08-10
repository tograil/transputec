using AutoMapper;
using CrisesControl.Api.Application.Commands.Billing.GetAllInvoices;
using CrisesControl.Api.Application.Commands.Billing.GetBillingSummary;
using CrisesControl.Api.Application.Commands.Billing.GetInvSchedule;
using CrisesControl.Api.Application.Commands.Billing.GetOrders;
using CrisesControl.Api.Application.Commands.Billing.GetPaymentProfile;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Billing.Repositories;

namespace CrisesControl.Api.Application.Query {
    public class BillingQuery : IBillingQuery  {
        private readonly IBillingRepository _billingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BillingQuery> _logger;

        public BillingQuery(IBillingRepository billingRepository, IMapper mapper,
            ILogger<BillingQuery> logger) {
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

    }
}
