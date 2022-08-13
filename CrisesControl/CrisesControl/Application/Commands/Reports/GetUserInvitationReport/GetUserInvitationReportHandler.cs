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
            throw new NotImplementedException();
        }
    }
}
