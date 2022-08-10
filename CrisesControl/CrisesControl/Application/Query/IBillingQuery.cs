using CrisesControl.Api.Application.Commands.Billing.GetAllInvoices;
using CrisesControl.Api.Application.Commands.Billing.GetBillingSummary;
using CrisesControl.Api.Application.Commands.Billing.GetInvSchedule;
using CrisesControl.Api.Application.Commands.Billing.GetOrders;
using CrisesControl.Api.Application.Commands.Billing.GetPaymentProfile;

namespace CrisesControl.Api.Application.Query {
    public interface IBillingQuery {
        Task<GetPaymentProfileResponse> GetPaymentProfile(GetPaymentProfileRequest request);
        Task<List<GetBillingSummaryResponse>> GetBillingSummary(GetBillingSummaryRequest request);
        Task<GetAllInvoicesResponse> GetAllInvoices(GetAllInvoicesRequest request);
        Task<List<GetInvScheduleResponse>> GetInvSchedule(GetInvScheduleRequest request);
        Task<List<GetOrdersResponse>> GetOrders(GetOrdersRequest request);
    }
}
