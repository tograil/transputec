using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetUserInvitationReport
{
    public class GetUserInvitationReportHandler:IRequestHandler<GetUserInvitationReportRequest, GetUserInvitationReportResponse>
    {
        private readonly IMapper _mapper;
        private readonly IReportsQuery _reportsQuery;
        public GetUserInvitationReportHandler(IMapper mapper, IReportsQuery reportsQuery)
        {
            _mapper = mapper;
            _reportsQuery = reportsQuery;
        }

        public Task<GetUserInvitationReportResponse> Handle(GetUserInvitationReportRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetUserInvitationReportRequest));

            var response = _reportsQuery.GetUserInvitationReport(request);
            return response;
        }
    }
}
