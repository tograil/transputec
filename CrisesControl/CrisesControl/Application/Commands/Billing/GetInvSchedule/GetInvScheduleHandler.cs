using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Billing.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetInvSchedule
{
    public class GetInvScheduleHandler : IRequestHandler<GetInvScheduleRequest, GetInvScheduleResponse>
    {

        private readonly IBillingRepository _billingRepository;

        public GetInvScheduleHandler(IBillingRepository billingRepository)
        {
            _billingRepository = billingRepository;
        }

        public async Task<GetInvScheduleResponse> Handle(GetInvScheduleRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetInvScheduleRequest));

            var invoiceItems = await _billingRepository.GetInvItems(request.OrderId, request.MonthVal, request.YearVal);
            var response = new GetInvScheduleResponse();
            response.Response = invoiceItems;
            return response;
        }
    }
}
