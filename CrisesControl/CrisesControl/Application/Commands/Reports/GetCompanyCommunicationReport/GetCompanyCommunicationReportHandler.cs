using Ardalis.GuardClauses;
using CrisesControl.Core.Reports.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetCompanyCommunicationReport
{
    public class GetCompanyCommunicationReportHandler : IRequestHandler<GetCompanyCommunicationReportRequest, GetCompanyCommunicationReportResponse>
    {
        private readonly IReportsRepository _reportsRepository;
        public GetCompanyCommunicationReportHandler(IReportsRepository reportsRepository)
        {
            _reportsRepository = reportsRepository;
        }
        public Task<GetCompanyCommunicationReportResponse> Handle(GetCompanyCommunicationReportRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetCompanyCommunicationReportRequest));

            var re
        }
    }
}
