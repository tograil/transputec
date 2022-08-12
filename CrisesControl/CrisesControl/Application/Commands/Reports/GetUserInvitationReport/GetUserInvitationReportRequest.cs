using CrisesControl.Core.Compatibility;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetUserInvitationReport
{
    public class GetUserInvitationReportRequest : CcBase, IRequest<GetUserInvitationReportResponse>
    {

    }
}
