using Ardalis.GuardClauses;
using CrisesControl.Core.Billing.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetUsageGraph
{
    public class GetUsageGraphHandler : IRequestHandler<GetUsageGraphRequest, GetUsageGraphResponse>
    {
        private readonly IBillingRepository _billingRepository;
        public GetUsageGraphHandler(IBillingRepository billingRepository)
        {
            _billingRepository = billingRepository;
        }

        public async Task<GetUsageGraphResponse> Handle(GetUsageGraphRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));
            var response = new GetUsageGraphResponse();
            response.Result = await _billingRepository.GetUsageGraph(request.CompanyId, request.ReportType, request.LastMonth);
            return response;
        }
    }
}
