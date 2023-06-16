using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.ExportUserInvitationDump
{
    public class ExportUserInvitationDumpHandler:IRequestHandler<ExportUserInvitationDumpRequest, ExportUserInvitationDumpResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        public ExportUserInvitationDumpHandler(IReportsQuery reportsQuery)
        {
            _reportsQuery = reportsQuery;
        }

        public async Task<ExportUserInvitationDumpResponse> Handle(ExportUserInvitationDumpRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ExportUserInvitationDumpRequest));

            var response = _reportsQuery.ExportUserInvitationDump(request);
            return response;
        }
    }
}
