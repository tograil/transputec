﻿using CrisesControl.Api.Application.Commands.Billing.GetAllInvoices;
using CrisesControl.Api.Application.Commands.Billing.GetBillingSummary;
using CrisesControl.Api.Application.Commands.Billing.GetInvoicesById;
using CrisesControl.Api.Application.Commands.Billing.GetInvSchedule;
using CrisesControl.Api.Application.Commands.Billing.GetOrders;
using CrisesControl.Api.Application.Commands.Billing.GetPaymentProfile;
using CrisesControl.Api.Application.Commands.Billing.GetTransactionDetails;
using CrisesControl.Api.Application.Commands.Billing.GetUnbilledSummary;
using CrisesControl.Api.Application.Commands.Billing.GetUsageGraph;

namespace CrisesControl.Api.Application.Query {
    public interface IBillingQuery {
        Task<GetPaymentProfileResponse> GetPaymentProfile(GetPaymentProfileRequest request);
        Task<List<GetBillingSummaryResponse>> GetBillingSummary(GetBillingSummaryRequest request);
        Task<GetAllInvoicesResponse> GetAllInvoices(GetAllInvoicesRequest request);
        Task<List<GetInvScheduleResponse>> GetInvSchedule(GetInvScheduleRequest request);
        Task<List<GetOrdersResponse>> GetOrders(GetOrdersRequest request);
        Task<GetInvoicesByIdResponse> GetInvoicesById(GetInvoicesByIdRequest request);
        Task<List<GetTransactionDetailsResponse>> GetTransactionDetails(GetTransactionDetailsRequest request);
        Task<List<GetUsageGraphResponse>> GetUsageGraph(GetUsageGraphRequest request);
        Task<List<GetUnbilledSummaryResponse>> GetUnbilledSummary(GetUnbilledSummaryRequest request);
    }
}
