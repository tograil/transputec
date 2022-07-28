using Ardalis.GuardClauses;
using CrisesControl.Core.Billing;
using CrisesControl.Core.Billing.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetUnbilledSummary
{
    public class GetUnbilledSummaryHandler : IRequestHandler<GetUnbilledSummaryRequest, GetUnbilledSummaryResponse>
    {
        private readonly IBillingRepository _billingRepository;
        public GetUnbilledSummaryHandler(IBillingRepository billingRepository)
        {
            _billingRepository = billingRepository;
        }

        public async Task<GetUnbilledSummaryResponse> Handle(GetUnbilledSummaryRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUnbilledSummaryRequest));
            var unbilledSummary = new List<UnbilledSummary>();

            if (request.ReportType.ToUpper() == "SUMMARY")
            {
                unbilledSummary = await _billingRepository.GetUnbilledSummary(request.StartYear);
            }
            else if (request.ReportType.ToUpper() == "MONTH")
            {
                unbilledSummary =await _billingRepository.GetUnbilledSummaryByMonth(request.StartYear, request.StartMonth);
            }
            else if (request.ReportType.ToUpper() == "MESSAGE")
            {
                unbilledSummary =await _billingRepository.GetUnbilledSummaryByMessage(request.MessageId);
            }
            var result = new GetUnbilledSummaryResponse();
            result.Response = unbilledSummary;
            return result;
        }
    }
}
